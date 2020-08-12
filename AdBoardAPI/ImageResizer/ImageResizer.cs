using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AdBoardAPI.ImageResizer
{
    public class ImageResizer
    {
        private HttpRequest _request;
        private ResizeParameters _resizeParams;
        private readonly ILogger<ImageResizer> _logger;
        private readonly PathString _path;
        private readonly IMemoryCache _memoryCache;
        private readonly DateTime _lastWriteTimeUtc;
        private readonly IWebHostEnvironment _env;

        public ImageResizer(HttpRequest request, ILoggerFactory factory, IMemoryCache memoryCache, IWebHostEnvironment env)
        {
            _request = request;
            _path = _request.Path;
            _logger = factory.CreateLogger<ImageResizer>();
            _memoryCache = memoryCache;
            _env = env;
        }

        public SKData Resize()
        {

            _resizeParams = GetResizeParams();
            if (_resizeParams is null)
            {
                return null;
            }

            SKData imageData = null;

            try
            {
                imageData = GetImageData();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex.Message);
            }

            return imageData;
        }

        private ResizeParameters GetResizeParams()
        {
            var resizeParams = new ResizeParameters();
            var query = _request.Query;

            resizeParams.HasParams =
                resizeParams.GetType().GetTypeInfo()
                    .GetProperties().Where(f => f.Name != "HasParams")
                    .Any(f => query.ContainsKey(f.Name));

            if (!resizeParams.HasParams)
            {
                return null;
            }

            var width = 0;
            if (query.ContainsKey("width"))
            {
                int.TryParse(query["width"], out width);
            }
            resizeParams.Width = width;

            var height = 0;
            if (query.ContainsKey("height"))
            {
                int.TryParse(query["height"], out height);
            }
            resizeParams.Height = height;

            var format = _path.Value.Substring(_path.Value.LastIndexOf('.') + 1);
            resizeParams.format = format;

            return resizeParams;
        }

        private SKData GetImageData()
        {
            var imageFilePath = Path.Combine(_env.WebRootPath, _path.Value.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar));

            var lastWriteTimeUtc = File.GetLastWriteTimeUtc(imageFilePath);
            if (lastWriteTimeUtc.Year == 1601)
            {
                return null;
            }

            long cacheKey;

            unchecked
            {
                cacheKey = _path.GetHashCode() + _lastWriteTimeUtc.ToBinary() +
                           _resizeParams.ToString().GetHashCode();
            }

            var isCached = _memoryCache.TryGetValue<byte[]>(cacheKey, out var imageBytes);
            if (isCached)
            {
                _logger.LogInformation("Изображение восстановлено из кэша.");
                return SKData.CreateCopy(imageBytes);
            }

            var bitmap = LoadBitmap(File.OpenRead(imageFilePath), out _);

            if (_resizeParams.Height == 0)
            {
                _resizeParams.Height = (int) Math.Round(bitmap.Height * (float) _resizeParams.Width / bitmap.Width);
            }
            else if (_resizeParams.Width == 0)
            {
                _resizeParams.Width = (int) Math.Round(bitmap.Width * (float) _resizeParams.Height / bitmap.Height);
            }

            var resizedImageInfo = new SKImageInfo(_resizeParams.Width, _resizeParams.Height,
                SKImageInfo.PlatformColorType, bitmap.AlphaType);
            using var resizedBitmap = bitmap.Resize(resizedImageInfo, SKFilterQuality.High);
            using var resizedImage = SKImage.FromBitmap(resizedBitmap);

            var encodeFormat = _resizeParams.format == "png" ? SKEncodedImageFormat.Png : SKEncodedImageFormat.Jpeg;
            var imageData = resizedImage.Encode(encodeFormat, 100);

            _memoryCache.Set(cacheKey, imageData.ToArray());
            bitmap.Dispose();

            return imageData;
        }

        private SKBitmap LoadBitmap(Stream stream, out SKEncodedOrigin origin)
        {
            using var managedStream = new SKManagedStream(stream);
            using var codec = SKCodec.Create(managedStream);

            origin = codec.EncodedOrigin;

            var info = codec.Info;
            var bitmap = new SKBitmap(info.Width, info.Height, SKImageInfo.PlatformColorType, info.IsOpaque ? SKAlphaType.Opaque : SKAlphaType.Premul);
            var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels(out _));

            if (result == SKCodecResult.Success || result == SKCodecResult.IncompleteInput)
            {
                return bitmap;
            }

            throw new ArgumentException("Невозможно загрузить изображение из источника.");
        }
    }
}
