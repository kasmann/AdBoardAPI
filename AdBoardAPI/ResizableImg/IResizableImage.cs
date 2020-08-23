using System.IO;

namespace AdBoardAPI.ResizableImg
{
    public interface IResizableImage
    {
        public FileStream ImageStream { get; }
    }
}
