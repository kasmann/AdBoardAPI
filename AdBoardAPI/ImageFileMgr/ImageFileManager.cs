using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AdBoardAPI.ImageFileMgr
{
    class ImageFileManager : IImageFileManager
    {
        private IConfiguration _configuration;

        public ImageFileManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateURL(string adId, string imageName)
        {
            var staticFilesRoot = _configuration.GetSection("AppConfiguration")["staticFilesRoot"].Trim().Replace(Path.DirectorySeparatorChar, '/');
            return Path.Combine(staticFilesRoot, $"{adId}-{imageName.Trim()}").Replace(Path.DirectorySeparatorChar, '/');
        }

        public async Task UploadImageAsync(IFormFile image, string path)
        {
            using var fileStream = new FileStream(path, FileMode.Create);
            await image.CopyToAsync(fileStream);

        }
    }
}
