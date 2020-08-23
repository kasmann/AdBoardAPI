using AdBoardAPI.CustomCache.CustomCacheInfo;

namespace AdBoardAPI.CustomCache.CustomCacheController
{
    public interface ICustomImageCacheController
    {
        public void CheckCacheState(ICustomImageCacheInfo cacheInfo);
        public void ClearCache(string cacheRoot);
    }
}
