namespace AdBoardAPI.CustomCache.CustomCacheInfo
{
    public interface ICustomImageCacheInfo
    {
        public string CacheRoot { get; set; }
        public uint MaxCacheSize { get; }
        public uint MaxFilesCached { get; }
    }
}
