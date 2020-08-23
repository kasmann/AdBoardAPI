using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace AdBoardAPI.ImageResizer
{
    public class ResizeParameters
    {
        public bool HasParams { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Format { get; set; }

        public ResizeParameters(PathString path, IQueryCollection parameters)
        {
            GetResizeParams(path, parameters);
        }

        private void GetResizeParams(PathString path, IQueryCollection parameters)
        {
            HasParams =
                typeof(ResizeParameters).GetTypeInfo()
                    .GetProperties().Where(f => f.Name != "HasParams")
                    .Any(f => parameters.ContainsKey(f.Name.ToLower()));

            var width = 0;
            if (parameters.ContainsKey("width"))
            {
                int.TryParse(parameters["width"], out width);
            }
            Width = width;

            var height = 0;
            if (parameters.ContainsKey("height"))
            {
                int.TryParse(parameters["height"], out height);
            }
            Height = height;
            Format = Path.GetExtension(path.Value);
        }

        public string ToUrlPart()
        {
            return $"{Width}x{Height}{Format}";
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Ширина: {Width}, ");
            sb.Append($"Высота: {Height}, ");
            sb.Append($"Формат: {Format}");

            return sb.ToString();
        }
    }
}
