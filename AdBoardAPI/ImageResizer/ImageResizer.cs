using AdBoardAPI.ResizableImg;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System;
using System.IO;

namespace AdBoardAPI.ImageResizer
{
    public class ImageResizer
    {
        private readonly ILogger<ImageResizer> _logger;
        private readonly IResizableImage _image;
        private readonly ResizeParameters _resizeParameters;

        public ImageResizer(IResizableImage image, ResizeParameters resizeParameters, ILogger<ImageResizer> logger)
        {
            _image = image;
            _resizeParameters = resizeParameters;
            _logger = logger;
        }

        public SKData Resize()
        {
            if (_resizeParameters is null)
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

        private SKData GetImageData()
        {
            var bitmap = LoadBitmap(_image.ImageStream, out _);

            if (_resizeParameters.Height == 0)
            {
                _resizeParameters.Height = (int) Math.Round(bitmap.Height * (float)_resizeParameters.Width / bitmap.Width);
            }
            else if (_resizeParameters.Width == 0)
            {
                _resizeParameters.Width = (int) Math.Round(bitmap.Width * (float)_resizeParameters.Height / bitmap.Height);
            }

            var resizedImageInfo = new SKImageInfo(_resizeParameters.Width, _resizeParameters.Height,
                SKImageInfo.PlatformColorType, bitmap.AlphaType);
            using var resizedBitmap = bitmap.Resize(resizedImageInfo, SKFilterQuality.High);
            using var resizedImage = SKImage.FromBitmap(resizedBitmap);

            var encodeFormat = _resizeParameters.Format == "png" ? SKEncodedImageFormat.Png : SKEncodedImageFormat.Jpeg;
            var imageData = resizedImage.Encode(encodeFormat, 100);

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
