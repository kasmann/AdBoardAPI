using AdBoardAPI.CustomCache.CustomCacheInfo;
using System.Threading.Tasks;

namespace AdBoardAPI.CustomCacheManager
{
    public interface ICustomImageCacheManager
    {
        public Task<byte[]> CacheFileAsync(byte[] imageBytes, string filename, string cacheKey);
        public Task<byte[]> ReadCachedFileAsync(string cacheKey);
        public bool Contains(string cacheKey);
    }
}
