using System;
using System.IO;

namespace AdBoardAPI.ResizableImg
{
    public interface IResizableImage : IDisposable
    {
        public FileStream ImageStream { get; }
    }
}
