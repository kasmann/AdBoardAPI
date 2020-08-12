using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AdBoardAPI.ImageResizer
{
    public class ImageResizerMiddleware
    {    
        private readonly RequestDelegate _next;
        private readonly ILoggerFactory _factory;
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _memoryCache;

        private static readonly string[] Suffixes = { ".png", ".jpg", ".jpeg" };

        public ImageResizerMiddleware(RequestDelegate next, IWebHostEnvironment env, ILoggerFactory factory, IMemoryCache memoryCache)
        {
            _next = next;
            _factory = factory;
            _env = env;
            _memoryCache = memoryCache;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var path = httpContext.Request.Path;
            if (httpContext.Request.Query.Count == 0 || !IsImagePath(path))
            {
                await _next.Invoke(httpContext);
                return;
            }

            var imageResizer = new ImageResizer(httpContext.Request, _factory, _memoryCache, _env);
            var result = imageResizer.Resize();
            if (!(result is null))
            {
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
            return Suffixes.Any(x => path.Value.EndsWith(x, StringComparison.OrdinalIgnoreCase));
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
