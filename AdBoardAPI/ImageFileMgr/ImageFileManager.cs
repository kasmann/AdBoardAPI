using System;
using AdBoardAPI.Options;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using AdBoardAPI.CustomCache.CustomCacheController;

namespace AdBoardAPI.ImageFileMgr
{
    public class ImageFileManager : IImageFileManager
    {
        private readonly AppConfiguration _options;
        private readonly ICustomImageCacheController _cacheController;

        public ImageFileManager(AppConfiguration options, ICustomImageCacheController cacheController)
        {
            _options = options;
            _cacheController = cacheController;
        }

        public string GenerateURL(string adId, string imageName)
        {
            var staticFilesRoot = _options.SystemOptions.StaticFilesRoot.Trim().Replace(Path.DirectorySeparatorChar, '/');
            return Path.Join(staticFilesRoot, $"{adId}-{imageName.Trim()}").Replace(Path.DirectorySeparatorChar, '/');
        }

        public async Task UploadImageAsync(IFormFile image, string path)
        {
            await using var fileStream = new FileStream(path, FileMode.Create);
            var cacheRoot = Path.Join(_options.CacheOptions.CacheRoot, Path.GetFileName(path).Replace(".", ""));
            _cacheController.ClearCache(cacheRoot);
            await image.CopyToAsync(fileStream);

        }
    }
}
