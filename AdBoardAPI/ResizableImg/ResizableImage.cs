using System;
using System.IO;

namespace AdBoardAPI.ResizableImg
{
    public class ResizableImage : IResizableImage
    {
        public FileStream ImageStream { get; }

        public ResizableImage(FileStream imageFileStream)
        {
            if (imageFileStream is null || !imageFileStream.CanRead)
            {
                throw new ArgumentException("Поток, содержащий изображение, не существует или не может быть прочитан");
            }
            ImageStream = imageFileStream;
        }
    }
}
