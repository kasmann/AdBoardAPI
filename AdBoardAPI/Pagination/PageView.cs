using AdBoardAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdBoardAPI
{
    /// <summary>
    /// Сущность "Страница". Содержит список объявлений, признаки наличия предыдущей и следующей страницы, общее количество страниц
    /// </summary>
    public class PageView
    {
        /// <summary>
        /// Номер текущей страницы
        /// </summary>
        public int Page { get; internal set; }

        /// <summary>
        /// Количество записей на странице
        /// </summary>
        public int OnPage { get; internal set; }

        /// <summary>
        /// Общее количество страниц
        /// </summary>
        public int TotalPages { get; internal set; }

        /// <summary>
        /// Признак наличия предыдущей страницы
        /// </summary>
        public bool HasPrevious { get; internal set; }

        /// <summary>
        /// Признак наличия следующей страницы
        /// </summary>
        public bool HasNext { get; internal set; }

        /// <summary>
        /// Список объявлений
        /// </summary>
        public List<Ad> List { get; internal set; }
    }
}