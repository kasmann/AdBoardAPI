using AdBoardAPI.ResizableImg;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdBoardAPI.ImageResizer
{
    public class ImageResizerMiddleware
    {    
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ImageResizerMiddleware> _logger;
        private readonly ILoggerFactory _factory;

        private static readonly string[] Suffixes = { ".png", ".jpg", ".jpeg" };

        public ImageResizerMiddleware(RequestDelegate next, IWebHostEnvironment env, ILoggerFactory factory, IMemoryCache memoryCache)
        {
            _next = next;
            _env = env;
            _memoryCache = memoryCache;
            _factory = factory;
            _logger = new Logger<ImageResizerMiddleware>(_factory);
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var path = httpContext.Request.Path;
            var resizeParameters = new ResizeParameters(path, httpContext.Request.Query);
            if (httpContext.Request.Query.Count == 0 || !IsImagePath(path) || !resizeParameters.HasParams)
            {
                await _next.Invoke(httpContext);
                return;
            }


            var imageFilePath = Path.Combine(_env.WebRootPath,
                path.Value.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar));
            var lastWriteTimeUtc = File.GetLastWriteTimeUtc(imageFilePath);


            if (lastWriteTimeUtc.Year == 1601)
            {
                await _next.Invoke(httpContext);
            }

            long cacheKey;
            unchecked
            {
                cacheKey = path.GetHashCode() + lastWriteTimeUtc.ToBinary() +
                           resizeParameters.ToString().GetHashCode();
            }

            SKData result;
            var isCached = _memoryCache.TryGetValue<byte[]>(cacheKey, out var imageBytes);
            if (isCached)
            {
                _logger.LogInformation("Изображение восстановлено из кэша.");
                result = SKData.CreateCopy(imageBytes);
                await httpContext.Response.Body.WriteAsync(result.ToArray(), 0, result.ToArray().Length);
                return;
            }

            IResizableImage image = new ResizableImage(File.OpenRead(path));
            var imageResizer = new ImageResizer(image, resizeParameters, new Logger<ImageResizer>(_factory));
            result = imageResizer.Resize();
            if (!(result is null))
            {
                _memoryCache.Set(cacheKey, result.ToArray());
                await httpContext.Response.Body.WriteAsync(result.ToArray(), 0, result.ToArray().Length);
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }

        private bool IsImagePath(PathString path)
        {
            if (path == null || !path.HasValue)
            {
                return false;
            }
            return Suffixes.Any(x => Path.GetExtension(path) == x);
        }
    }


    public static class ImageResizerMiddlewareExtensions
    {
        public static IServiceCollection AddImageResizer(this IServiceCollection services)
        {
            return services.AddMemoryCache();
        }

        public static IApplicationBuilder UseImageResizer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ImageResizerMiddleware>();
        }
    }
}
