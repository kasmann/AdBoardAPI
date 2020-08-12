using System.ComponentModel.DataAnnotations;
using System.IO;

namespace AdBoardAPI.Options
{
    public class SystemOptions
    {
        [Exists(ErrorMessage = "Указанная директория статических файлов не существует")]
        public string StaticFilesRoot { get; set; }
    }

    public class ExistsAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return Directory.Exists(value.ToString());
        }
    }
}
