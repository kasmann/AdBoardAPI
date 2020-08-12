using AdBoardAPI.Models;
using AdBoardAPI.Pagination.Sorter;
using System.Linq;

namespace AdBoardAPI.Pagination
{
    public class AdsSorter : ISorter<Ad>
    {
        private IQueryable<Ad> _list;
        private string _sortBy;
        public AdsSorter(IQueryable<Ad> list, string sortBy)
        {
            _list = list;
            _sortBy = sortBy;
        }
        public IQueryable<Ad> Sort()
        {
            _list = _sortBy switch
            {
                "id" => _list.OrderBy(ad => ad.Id.ToString()),
                "id-" => _list.OrderByDescending(ad => ad.Id.ToString()),
                "number-" => _list.OrderByDescending(ad => ad.Number),
                "user" => _list.OrderBy(ad => ad.User.ToString()),
                "user-" => _list.OrderByDescending(ad => ad.User.ToString()),
                "subject" => _list.OrderBy(ad => ad.Subject),
                "subject-" => _list.OrderByDescending(ad => ad.Subject),
                "rating" => _list.OrderBy(ad => ad.Rating),
                "rating-" => _list.OrderByDescending(ad => ad.Rating),
                "created" => _list.OrderBy(ad => ad.Created),
                "created-" => _list.OrderByDescending(ad => ad.Created),
                _ => _list.OrderBy(ad => ad.Number),
            };

            return _list;
        }
    }
}
