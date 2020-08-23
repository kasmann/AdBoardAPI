using System;
using System.IO;
using System.Threading.Tasks;
using AdBoardAPI.CustomCache.CustomCacheInfo;
using AdBoardAPI.CustomCacheManager;

namespace AdBoardAPI.CustomCache.CustomCacheManager
{
    public class PhysicalImageCacheManager : ICustomImageCacheManager
    {
        private readonly string _cacheRoot;

        public PhysicalImageCacheManager(ICustomImageCacheInfo cacheInfo)
        {
            _cacheRoot = cacheInfo is PhysicalImageCacheInfo info ? info.SpecCacheRoot : cacheInfo.CacheRoot;
        }

        public async Task<byte[]> CacheFileAsync(byte[] imageBytes, string filename, string cacheKey)
        {
            if (!Directory.Exists(_cacheRoot))
            {
                Directory.CreateDirectory(_cacheRoot);
            }

            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            var extension = Path.GetExtension(filename);
            var fileFullpath = Path.Join(_cacheRoot, $"{filenameWithoutExtension}_{cacheKey}{extension}");

            await File.WriteAllBytesAsync(fileFullpath, imageBytes);
            return await File.ReadAllBytesAsync(fileFullpath);
        }

        public async Task<byte[]> ReadCachedFileAsync(string cacheKey)
        {
            var filename = Directory.GetFiles(_cacheRoot, $"*_{cacheKey}.*")[0];
            return await File.ReadAllBytesAsync(filename);
        }

        public bool Contains(string cacheKey)
        {
            return Directory.Exists(_cacheRoot) 
                   && Directory.GetFiles(_cacheRoot, $"*_{cacheKey}.*").Length > 0;
        }

    }
}
