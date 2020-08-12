using AdBoardAPI.Models;
using AdBoardAPI.Pagination.Searcher;
using System.Linq;

namespace AdBoardAPI.Pagination
{
    public class AdsSearcher : ISearcher<Ad>
    {
        private IQueryable<Ad> _list;
        private string _search;

        public AdsSearcher(IQueryable<Ad> list, string search)
        {
            _list = list;
            _search = search;
        }

        public IQueryable<Ad> Search()
        {
            _list = _list.Where(ad => ad.Subject.Contains(_search) || ad.Content.Contains(_search) || ad.Number.ToString() == _search);
            return _list;
        }
    }
}
