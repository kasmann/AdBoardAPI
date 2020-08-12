using AdBoardAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdBoardAPI
{
    public class Paginator<T> where T : class, IModel
    {
        private IQueryable<T> _list;
        private readonly int? _page;
        private readonly int? _onPage;

        public Paginator(IQueryable<T> list, int? page, int? onPage)
        {
            _list = list;
            _page = page;
            _onPage = onPage;               
        }

        public async Task<PageView<T>> MakePageViewAsync()
        {
            List<T> list;
            int count = await _list.CountAsync();
            int page = _page.HasValue ? (_page.Value == 0 ? 1 : _page.Value) : 1;
            int onPage = _onPage.HasValue ? (_onPage.Value == 0 ? 10 : _onPage.Value) : 10;
            int totalPages;

            if (!_page.HasValue && !_onPage.HasValue)
            {
                page = 1;
                onPage = count;
                totalPages = 1;
                list = await _list.ToListAsync();
            }
            else
            {
                totalPages = (int)Math.Floor((double)(count / onPage)) + 1;
                list = await _list.Skip((page - 1) * onPage).Take(onPage).ToListAsync();
            }

            var pageView = new PageView<T>
            {
                Page = page,
                OnPage = onPage,
                TotalPages = totalPages,
                HasNext = (page < totalPages),
                HasPrevious = (page > 1),
                List = list
            };

            return pageView;
        }
    }
}
