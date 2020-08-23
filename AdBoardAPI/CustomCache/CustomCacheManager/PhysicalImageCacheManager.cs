using System;
using System.IO;
using System.Threading.Tasks;
using AdBoardAPI.CustomCache.CustomCacheInfo;
using AdBoardAPI.CustomCacheManager;

namespace AdBoardAPI.CustomCache.CustomCacheManager
{
    public class PhysicalImageCacheManager : ICustomImageCacheManager
    {
        private ICustomImageCacheInfo _cacheInfo;

        public event ICustomImageCacheManager.CacheHandler OnFileReadyToCache;

        public PhysicalImageCacheManager(ICustomImageCacheInfo cacheInfo)
        {
            _cacheInfo = cacheInfo;
        }

        public async Task<byte[]> CacheFileAsync(byte[] imageBytes, string filename, string cacheKey)
        {
            if (!Directory.Exists(_cacheInfo.CacheRoot))
            {
                Directory.CreateDirectory(_cacheInfo.CacheRoot);
            }

            OnFileReadyToCache?.Invoke(_cacheInfo);

            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            var extension = Path.GetExtension(filename);
            var fileFullpath = Path.Join(_cacheInfo.CacheRoot, $"{filenameWithoutExtension}_{cacheKey}{extension}");
            
            await File.WriteAllBytesAsync(fileFullpath, imageBytes);
            return await File.ReadAllBytesAsync(fileFullpath);
        }

        public async Task<byte[]> ReadCachedFileAsync(string cacheKey)
        {
            var filename = Directory.GetFiles(_cacheInfo.CacheRoot, $"*_{cacheKey}.*")[0];
            return await File.ReadAllBytesAsync(filename);
        }

        public bool Contains(string cacheKey)
        {
            return Directory.Exists(_cacheInfo.CacheRoot) 
                   && Directory.GetFiles(_cacheInfo.CacheRoot, $"*_{cacheKey}.*").Length > 0;
        }

    }
}
