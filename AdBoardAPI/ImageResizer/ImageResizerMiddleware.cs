using AdBoardAPI.CustomCache.CustomCacheController;
using AdBoardAPI.CustomCache.CustomCacheInfo;
using AdBoardAPI.CustomCache.CustomCacheManager;
using AdBoardAPI.CustomCacheManager;
using AdBoardAPI.Options;
using AdBoardAPI.ResizableImg;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdBoardAPI.ImageResizer
{
    public class ImageResizerMiddleware
    {    
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ImageResizer _imageResizer;
        private readonly AppConfiguration _appConfiguration;
        private readonly ICustomImageCacheController _cacheController;

        private static readonly string[] Suffixes = { ".png", ".jpg", ".jpeg" };

        public ImageResizerMiddleware(RequestDelegate next, IWebHostEnvironment env, ImageResizer imageResizer, 
                                        AppConfiguration appConfiguration, ICustomImageCacheController cacheController)
        {
            _next = next;
            _env = env;
            _imageResizer = imageResizer;
            _appConfiguration = appConfiguration;
            _cacheController = cacheController;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var path = httpContext.Request.Path.Value;
            var resizeParameters = new ResizeParameters(path, httpContext.Request.Query);
            if (httpContext.Request.Query.Count == 0 || !IsImagePath(path) || !resizeParameters.HasParams)
            {
                await _next.Invoke(httpContext);
                return;
            }

            var cacheKey = (path.GetHashCode() + resizeParameters.ToString().GetHashCode()).ToString("X");
            var cacheDirectoryName = Path.GetFileName(path).Replace(".", "");

            //сведения об общей директории кэша
            var cacheInfo = new PhysicalImageCacheInfo(_appConfiguration.CacheOptions);
            cacheInfo.SpecCacheRoot = Path.Join(cacheInfo.CacheRoot, cacheDirectoryName);
            
            ICustomImageCacheManager cacheManager = new PhysicalImageCacheManager(cacheInfo);

            if (cacheManager.Contains(cacheKey))
            {
                var cachedImageBytes = await cacheManager.ReadCachedFileAsync(cacheKey);
                await httpContext.Response.Body.WriteAsync(cachedImageBytes, 0, cachedImageBytes.Length);
                return;
            }
            
            await using var imageStream = File.OpenRead(path);
            using IResizableImage image = new ResizableImage(imageStream);
            
            var result = _imageResizer.Resize(image, resizeParameters);

            if (!(result is null))
            {
                _cacheController.CheckCacheState(cacheInfo);

                var cachedFile = await cacheManager.CacheFileAsync(result.ToArray(), path, cacheKey);
                await httpContext.Response.Body.WriteAsync(cachedFile, 0, cachedFile.Length);
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
        public static IServiceCollection AddImageResizer(this IServiceCollection services, ILoggerFactory factory)
        {
            return services.AddSingleton(new ImageResizer(new Logger<ImageResizer>(factory)));
        }

        public static IApplicationBuilder UseImageResizer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ImageResizerMiddleware>();
        }
    }
}
