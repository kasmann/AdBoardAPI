using System.ComponentModel.DataAnnotations;

namespace AdBoardAPI.Options
{
    public class PublishOptions
    {
        [Range(0, 100, ErrorMessage = "Значение {0} должно быть между {1} и {2}")]
        public int MaxAdsByUser { get; set; } = -1;
    }
}
