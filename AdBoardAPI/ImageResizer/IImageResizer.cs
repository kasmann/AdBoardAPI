using AdBoardAPI.ResizableImg;
using SkiaSharp;

namespace AdBoardAPI.ImageResizer
{
    public interface IImageResizer
    {
        public SKData Resize(IResizableImage image, ResizeParameters resizeParameters);
    }
}