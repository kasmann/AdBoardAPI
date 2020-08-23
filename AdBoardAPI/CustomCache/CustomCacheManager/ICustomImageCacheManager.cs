using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdBoardAPI.CustomCache.CustomCacheInfo;

namespace AdBoardAPI.CustomCacheManager
{
    public interface ICustomImageCacheManager
    {
        public Task<string> CacheFileAsync(byte[] imageBytes, string filename, string cacheKey);
        public Task<byte[]> ReadCachedFileAsync(string cacheKey);
        public bool Contains(string cacheKey);

        public delegate void CacheHandler(ICustomImageCacheInfo cacheInfo);

        public event CacheHandler OnFileReadyToCache;
    }
}
