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

            var mainCacheInfo = new PhysicalImageCacheInfo(_appConfiguration.CacheOptions);

            var specCacheInfo = new PhysicalImageCacheInfo(_appConfiguration.CacheOptions);
            specCacheInfo.CacheRoot = Path.Join(specCacheInfo.CacheRoot, cacheDirectoryName);

            ICustomImageCacheManager specCacheManager = new PhysicalImageCacheManager(specCacheInfo);

            if (specCacheManager.Contains(cacheKey))
            {
                var cachedImageBytes = await specCacheManager.ReadCachedFileAsync(cacheKey);
                await httpContext.Response.Body.WriteAsync(cachedImageBytes, 0, cachedImageBytes.Length);
                return;
            }
            
            await using var imageStream = File.OpenRead(path);
            using IResizableImage image = new ResizableImage(imageStream);
            
            var result = _imageResizer.Resize(image, resizeParameters);

            if (!(result is null))
            {
                //проверить общее состояние кэша
                _cacheController.CheckCacheState(mainCacheInfo);
                
                specCacheManager.OnFileReadyToCache += _cacheController.CheckCacheState;
                var cachedFilepath = await specCacheManager.CacheFileAsync(result.ToArray(), path, cacheKey);
                var cachedFile = File.ReadAllBytesAsync(cachedFilepath);
                await httpContext.Response.Body.WriteAsync(await cachedFile, 0, cachedFile.Result.Length);
                specCacheManager.OnFileReadyToCache -= _cacheController.CheckCacheState;
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
