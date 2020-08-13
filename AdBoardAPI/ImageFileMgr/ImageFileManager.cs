using AdBoardAPI.Options;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace AdBoardAPI.ImageFileMgr
{
    public class ImageFileManager : IImageFileManager
    {
        private readonly AppConfiguration _options;

        public ImageFileManager(AppConfiguration options)
        {
            _options = options;
        }

        public string GenerateURL(string adId, string imageName)
        {
            var staticFilesRoot = _options.SystemOptions.StaticFilesRoot.Trim().Replace(Path.DirectorySeparatorChar, '/');
            return Path.Combine(staticFilesRoot, $"{adId}-{imageName.Trim()}").Replace(Path.DirectorySeparatorChar, '/');
        }

        public async Task UploadImageAsync(IFormFile image, string path)
        {
            await using var fileStream = new FileStream(path, FileMode.Create);
            await image.CopyToAsync(fileStream);

        }
    }
}
