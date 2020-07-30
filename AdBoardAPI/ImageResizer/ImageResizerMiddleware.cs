using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AdBoardAPI.Controllers;

namespace AdBoardAPI.ImageResizer
{
    public class ImageResizerMiddleware
    {    
        private readonly RequestDelegate _next;
        ILoggerFactory _factory;
        IWebHostEnvironment _env;

        private static readonly string[] suffixes = new string[] { ".png", ".jpg", ".jpeg" };

        public ImageResizerMiddleware(RequestDelegate next, IWebHostEnvironment env, ILoggerFactory factory, IMemoryCache memoryCache)
        {
            _next = next;
            _factory = factory;
            _env = env;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var path = httpContext.Request.Path;
            if (httpContext.Request.Query.Count == 0 || !IsImagePath(path))
            {
                await _next.Invoke(httpContext);
                return;
            }

            var resizeParams = GetResizeParams(path, httpContext.Request.Query);
            if (!resizeParams.hasParams)
            {
                await _next.Invoke(httpContext);
                return;
            }

            var imageResizer = new ImageResizerFake(path, resizeParams, _env, _factory);
            var result = imageResizer.Resize();           
            
            await httpContext.Response.Body.WriteAsync(result, 0, result.Length);
        }

        private bool IsImagePath(PathString path)
        {
            if (path == null || !path.HasValue)
            {
                return false;
            }
            return suffixes.Any(x => path.Value.EndsWith(x, StringComparison.OrdinalIgnoreCase));
        }

        private ResizeParameters GetResizeParams(PathString path, IQueryCollection query)
        {
            ResizeParameters resizeParams = new ResizeParameters();

            resizeParams.hasParams =
                resizeParams.GetType().GetTypeInfo()
                .GetFields().Where(f => f.Name != "hasParams")
                .Any(f => query.ContainsKey(f.Name));

            if (!resizeParams.hasParams)
                return resizeParams;

            if (query.ContainsKey("autorotate"))
                resizeParams.autorotate = true;

            int width = 0;
            if (query.ContainsKey("width"))
                int.TryParse(query["width"], out width);
            resizeParams.width = width;

            int height = 0;
            if (query.ContainsKey("height"))
                int.TryParse(query["height"], out height);
            resizeParams.height = height;

            string format = path.Value.Substring(path.Value.LastIndexOf('.') + 1);
            resizeParams.format = format;

            return resizeParams;
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
