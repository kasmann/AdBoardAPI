using System;
using System.ComponentModel.DataAnnotations;

namespace AdBoardAPI.Models
{
    /// <summary>
    /// Сущность "Объявление".
    /// </summary>
    public class Ad
    {
        /// <summary>
        /// Уникальный идентификатор объявления. Генерируется при публикации. Ключевое поле
        /// </summary>
        /// <example>a24b9f8c-c41f-456f-9c1f-2bb6f35252b0</example>
        public Guid Id { get; internal set; }

        /// <summary>
        /// Порядковый номер объявления
        /// </summary>
        /// <example>11</example>
        public int Number { get; internal set; }

        /// <summary>
        /// Уникальный идентификатор пользователя, опубликовавшего объявление
        /// </summary>
        /// <example>dacfb01c-2cb0-4321-bea4-42b3f238d85a</example>
        public Guid User { get; set; }

        /// <summary>
        /// Текст объявления
        /// </summary>
        /// <example>Продам телевизор Grundig 32"</example>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// Полноразмерная фотография
        /// </summary>
        public string ImageFullsize { get; set; }

        /// <summary>
        /// Превью фотографии
        /// </summary>
        public string ImagePreview { get; internal set; }

        /// <summary>
        /// Рейтинг объявления. Назначается системой
        /// </summary> 
        /// <example>-3</example>
        public int Rating { get; internal set; }

        /// <summary>
        /// Дата публикации объявления 
        /// </summary> 
        /// <example>2020-07-23T14:44:12</example>
        public DateTime Created { get; internal set; }
    }
}