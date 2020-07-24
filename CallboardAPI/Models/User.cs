using System;
using System.ComponentModel.DataAnnotations;

namespace AdBoardAPI.Models
{
    /// <summary>
    /// Сущность "Пользователь".
    /// </summary>
    public class User
    {
        /// <summary>
        /// Уникальный идентификатор пользователя 
        /// </summary>
        /// <example>dacfb01c-2cb0-4321-bea4-42b3f238d85a</example>
        public Guid Id { get; internal set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        /// <example>Иванов И.И.</example>
        [Required]
        public string Name { get; internal set; }
    }
}