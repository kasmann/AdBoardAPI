using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.IO;

namespace AdBoardAPI.ImageResizer
{
    public class ImageResizerFake
    {
        private ResizeParameters _resizeParams;
        ILogger<ImageResizerFake> _logger;
        IWebHostEnvironment _env;
        PathString _path;

        public ImageResizerFake(PathString path, ResizeParameters resizeParams, IWebHostEnvironment env, ILoggerFactory factory)
        {
            _env = env;
            _logger = factory.CreateLogger<ImageResizerFake>();
            _resizeParams = resizeParams;
            _path = path;
        }

        public byte[] Resize()
        {
            _logger.LogInformation($"Преобразуем картинку {_path} к параметрам:" +
                $"\n\tширина {_resizeParams.width}" +
                $"\n\tвысота {_resizeParams.height}" +
                $"\n\tавтоповорот {_resizeParams.autorotate}");

            var imagePath = Path.Combine(_env.WebRootPath, _path.Value.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar));
            var buffer = File.ReadAllBytes(imagePath);
            return buffer;
        }
    }
}
