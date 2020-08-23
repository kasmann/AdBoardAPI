using System.ComponentModel.DataAnnotations;

namespace AdBoardAPI.Options
{
    public class CacheOptions
    {
        [Exists(ErrorMessage = "Указанная директория статических файлов не существует")]
        public string CacheRoot { get; set; }

        [Range(1, 1024, ErrorMessage = "Объем кэша {0} должен быть между {1} и {2}")]
        public uint MaxCacheSize { get; set; }

        [Range(1, 10000, ErrorMessage = "Максимальное количество кэшированных файлов {0} должно быть между {1} и {2}")]
        public uint MaxFilesCached { get; set; }
    }
}
