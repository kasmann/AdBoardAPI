using AdBoardAPI.Models;
using AdBoardAPI.Pagination.Filter;
using System;
using System.Linq;

namespace AdBoardAPI.Pagination
{
    public class AdsFilter : IFilter<Ad>
    {
        private IQueryable<Ad> _list;
        private string _user;
        private string _subject;
        private string _content;
        private int? _rating;
        private DateTime? _created;

        public AdsFilter(IQueryable<Ad> list, string user, string subject, string content, int? rating, DateTime? created)
        {
            _list = list;
            _user = user;
            _subject = subject;
            _content = content;
            _rating = rating;
            _created = created;   
        }

        public IQueryable<Ad> Filter()
        {
            if (!string.IsNullOrEmpty(_user))
            {
                _list = _list.Where(ad => ad.User.ToString() == _user);
            }
            if (!string.IsNullOrEmpty(_subject))
            {
                _list = _list.Where(ad => ad.Subject.Contains(_subject));
            }
            if (!string.IsNullOrEmpty(_content))
            {
                _list = _list.Where(ad => ad.Content.Contains(_content));
            }
            if (_rating.HasValue)
            {
                _list = _list.Where(ad => ad.Rating == _rating.Value);
            }
            if (_created.HasValue)
            {
                _list = _list.Where(ad => ad.Created >= _created.Value.Date && ad.Created < _created.Value.Date.AddDays(1));
            }

            return _list;
        }
    }
}
