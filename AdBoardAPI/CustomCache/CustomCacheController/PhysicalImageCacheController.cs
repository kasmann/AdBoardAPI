using AdBoardAPI.CustomCache.CustomCacheInfo;
using System;
using System.IO;
using System.Linq;
using AdBoardAPI.CustomCacheManager;

namespace AdBoardAPI.CustomCache.CustomCacheController
{
    public class PhysicalImageCacheController : ICustomImageCacheController
    {
        private ICustomImageCacheInfo _cacheInfo;

        public void CheckCacheState(ICustomImageCacheInfo cacheInfo)
        {
            _cacheInfo = cacheInfo;
            if (IsMaxSizeExceeded() || IsMaxFilesCountExceeded())
            {
                ClearCache(_cacheInfo.CacheRoot);
            }
        }

        public void ClearCache(string cacheRoot)
        {
            if (!Directory.Exists(cacheRoot)) return;

            var files = new DirectoryInfo(cacheRoot).GetFiles();
            foreach (var file in files)
            {
                file.Delete();
            }
        }

        private bool IsMaxSizeExceeded()
        {
            if (!Directory.Exists(_cacheInfo.CacheRoot)) return false;
            var cacheSize = new DirectoryInfo(_cacheInfo.CacheRoot).GetFiles().Sum(x => x.Length);
            var cacheSizeInMb = cacheSize / 1024 / 1024;

            return cacheSizeInMb > _cacheInfo.MaxCacheSize;
        }

        private bool IsMaxFilesCountExceeded()
        {
            if (!Directory.Exists(_cacheInfo.CacheRoot)) return false;
            var filesCount = new DirectoryInfo(_cacheInfo.CacheRoot).GetFiles().Length;

            return filesCount > _cacheInfo.MaxFilesCached;
        }
    }
}
