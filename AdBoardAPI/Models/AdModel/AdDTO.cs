using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AdBoardAPI.Models.AdModel
{
    /// <summary>
    /// Объект передачи данных для сущности "Объявление"
    /// </summary>
    public class AdDTO
    {
        /// <summary>
        /// Уникальный идентификатор пользователя, опубликовавшего объявление
        /// </summary>
        [Required]
        public Guid User { get; set; }

        /// <summary>
        /// Заголовок объявления
        /// </summary>
        /// <example>Продам телевизор Grundig 32"</example>
        [Required]
        public string Subject { get; set; }

        /// <summary>
        /// Текст объявления. Если текст не задан, то будет скопирован заголовок объявления
        /// </summary>
        /// <example>Продам телевизор Grundig 32" в хорошем состоянии</example>
        public string Content { get; set; }

        /// <summary>
        /// Полноразмерная фотография
        /// </summary>
        public IFormFile Image { get; set; }
    }
}