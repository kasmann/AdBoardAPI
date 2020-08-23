using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdBoardAPI.CustomCache.CustomCacheInfo
{
    public interface ICustomImageCacheInfo
    {
        public string CacheRoot { get; set; }
        public uint MaxCacheSize { get; }
        public uint MaxFilesCached { get; }
    }
}
