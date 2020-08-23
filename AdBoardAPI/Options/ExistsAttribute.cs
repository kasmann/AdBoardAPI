using System.ComponentModel.DataAnnotations;
using System.IO;

namespace AdBoardAPI.Options
{
    public class ExistsAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return Directory.Exists(value.ToString());
        }
    }
}
