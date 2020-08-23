using AdBoardAPI.CustomCache.CustomCacheInfo;
using System.IO;
using System.Linq;

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

            var dirs = new DirectoryInfo(cacheRoot).GetDirectories();
            foreach (var dir in dirs)
            {
                dir.Delete(true);
            }

            var files = new DirectoryInfo(cacheRoot).GetFiles();
            foreach (var file in files)
            {
                file.Delete();
            }
        }

        private bool IsMaxSizeExceeded()
        {
            if (!Directory.Exists(_cacheInfo.CacheRoot)) return false;

            var cacheSizeInMb = SumSize(new DirectoryInfo(_cacheInfo.CacheRoot)) / 1024 / 1024;
            
            return cacheSizeInMb > _cacheInfo.MaxCacheSize;
        }

        private bool IsMaxFilesCountExceeded()
        {
            if (!Directory.Exists(_cacheInfo.CacheRoot)) return false;

            var filesCount = SumCount(new DirectoryInfo(_cacheInfo.CacheRoot));
            
            return filesCount > _cacheInfo.MaxFilesCached;
        }

        private double SumSize(DirectoryInfo directoryInfo)
        {
            var subfoldersArray = directoryInfo.GetDirectories();

            if (subfoldersArray.Length == 0)
            {
                return directoryInfo.GetFiles().Sum(x => x.Length);
            }

            foreach (var subfolder in subfoldersArray)
            {
                return SumSize(subfolder);
            }

            return 0;
        }

        private int SumCount(DirectoryInfo directoryInfo)
        {
            var totalSum = directoryInfo.GetFiles().Length;

            var subfoldersArray = directoryInfo.GetDirectories();

            if (subfoldersArray.Length == 0)
            {
                return totalSum;
            }

            foreach (var subfolder in subfoldersArray)
            {
                totalSum += SumCount(subfolder);
            }

            return totalSum;
        }
    }
}
