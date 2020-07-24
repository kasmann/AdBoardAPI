using System;
using System.ComponentModel.DataAnnotations;

namespace AdBoardAPI.Models
{
    
    /// <summary>
    /// Объект передачи данных для сущности "Объявление"
    /// </summary>
    public class AdDTO
    {
        /// <summary>
        /// Уникальный идентификатор пользователя, опубликовавшего объявление
        /// </summary>
        public Guid User { get; set; }

        /// <summary>
        /// Текст объявления.
        /// </summary>
        /// <example>Продам телевизор Grundig 32"</example>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// Полноразмерная фотография
        /// </summary>
        public string ImageFullsize { get; set; }
    }
}