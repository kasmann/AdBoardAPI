using System.Text;

namespace AdBoardAPI.ImageResizer
{
    public class ResizeParameters
    {
        public bool HasParams { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        internal string format;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"ширина: {Width}, ");
            sb.Append($"высота: {Height}");

            return sb.ToString();
        }
    }
}
