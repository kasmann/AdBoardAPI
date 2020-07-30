using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdBoardAPI.ImageResizer
{
    public class ResizeParameters
    {
        public bool hasParams;
        public int width;
        public int height;
        public bool autorotate;
        internal string format;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"width: {width}, ");
            sb.Append($"height: {height}, ");
            sb.Append($"autorotate: {autorotate}, ");

            return sb.ToString();
        }
    }
}
