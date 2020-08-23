using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdBoardAPI.CustomCache.CustomCacheInfo;
using AdBoardAPI.CustomCacheManager;

namespace AdBoardAPI.CustomCache.CustomCacheController
{
    public interface ICustomImageCacheController
    {
        public void CheckCacheState(ICustomImageCacheInfo cacheInfo);
        public void ClearCache(string cacheRoot);
    }
}
