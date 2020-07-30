using AdBoardAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdBoardAPI
{
    public class Paginator
    {
        private readonly AdBoardContext _context;
        private IQueryable<Ad> _adsSet;
        private readonly string _user;
        private readonly string _subject;
        private readonly string _content;
        private readonly int? _rating;
        private readonly DateTime? _created;
        private readonly string _search;
        private readonly string _sortBy;
        private readonly int? _page;
        private readonly int? _onPage;

        public Paginator(AdBoardContext context, string user, string subject, string content, int? rating, DateTime? created, string search, string sortBy, int? page, int? onPage)
        {
            _context = context;
            _user = user;
            _subject = subject;
            _content = content;
            _rating = rating;
            _created = created;
            _search = search;
            _sortBy = sortBy;
            _page = page;
            _onPage = onPage;               
        }

        public async Task<PageView> MakePageAsync()
        {
            MakeList();
            if (string.IsNullOrEmpty(_search))
            {
                Filter();
            }
            else
            {
                Search();
            }
            Sort();
            return await MakePageView();
        }

        private void MakeList()
        {
            _adsSet = _context.Ads.Select(ad => ad);
        }

        private void Search()
        {
            _adsSet = _adsSet.Where(ad => ad.Subject.Contains(_search) || ad.Content.Contains(_search) || ad.Number.ToString() == _search);
        }

        private void Filter()
        {
            if (!string.IsNullOrEmpty(_user))
            {
                _adsSet = _adsSet.Where(ad => ad.User.ToString() == _user);
            }
            if (!string.IsNullOrEmpty(_subject))
            {
                _adsSet = _adsSet.Where(ad => ad.Subject.Contains(_subject));
            }
            if (!string.IsNullOrEmpty(_content))
            {
                _adsSet = _adsSet.Where(ad => ad.Content.Contains(_content));
            }
            if (_rating.HasValue)
            {
                _adsSet = _adsSet.Where(ad => ad.Rating == _rating.Value);
            }
            if (_created.HasValue)
            {
                _adsSet = _adsSet.Where(ad => ad.Created >= _created.Value.Date && ad.Created < _created.Value.Date.AddDays(1));
            }
        }

        private void Sort()
        {
            _adsSet = _sortBy switch
            {
                "id" => _adsSet.OrderBy(ad => ad.Id.ToString()),
                "id-" => _adsSet.OrderByDescending(ad => ad.Id.ToString()),
                "number-" => _adsSet.OrderByDescending(ad => ad.Number),
                "user" => _adsSet.OrderBy(ad => ad.User.ToString()),
                "user-" => _adsSet.OrderByDescending(ad => ad.User.ToString()),
                "subject" => _adsSet.OrderBy(ad => ad.Subject),
                "subject-" => _adsSet.OrderByDescending(ad => ad.Subject),
                "rating" => _adsSet.OrderBy(ad => ad.Rating),
                "rating-" => _adsSet.OrderByDescending(ad => ad.Rating),
                "created" => _adsSet.OrderBy(ad => ad.Created),
                "created-" => _adsSet.OrderByDescending(ad => ad.Created),
                _ => _adsSet.OrderBy(ad => ad.Number),
            };
        }

        private async Task<PageView> MakePageView()
        {
            List<Ad> list;
            int count = await _adsSet.CountAsync();
            int page = _page.HasValue ? (_page.Value == 0 ? 1 : _page.Value) : 1;
            int onPage = _onPage.HasValue ? (_onPage.Value == 0 ? 10 : _onPage.Value) : 10;
            int totalPages;

            if (!_page.HasValue && !_onPage.HasValue)
            {
                page = 1;
                onPage = count;
                totalPages = 1;
                list = await _adsSet.ToListAsync();
            }
            else
            {
                totalPages = (int)Math.Floor((double)(count / onPage)) + 1;
                list = await _adsSet.Skip((page - 1) * onPage).Take(onPage).ToListAsync();
            }

            var pageView = new PageView
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
