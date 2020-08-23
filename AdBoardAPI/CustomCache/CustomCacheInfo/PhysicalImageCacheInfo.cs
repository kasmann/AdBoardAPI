using AdBoardAPI.Options;

namespace AdBoardAPI.CustomCache.CustomCacheInfo
{
    public class PhysicalImageCacheInfo : ICustomImageCacheInfo
    {
        public string CacheRoot { get; set; }
        public string SpecCacheRoot { get; set; }
        public uint MaxCacheSize { get; }
        public uint MaxFilesCached { get; }

        public PhysicalImageCacheInfo(CacheOptions cacheOptions)
        {
            CacheRoot = cacheOptions.CacheRoot;
            MaxCacheSize = cacheOptions.MaxCacheSize;
            MaxFilesCached = cacheOptions.MaxFilesCached;
        }
    }
}
