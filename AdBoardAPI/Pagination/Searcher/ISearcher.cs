using AdBoardAPI.Models;
using System.Linq;

namespace AdBoardAPI.Pagination.Searcher
{
    interface ISearcher<T> where T : class, IModel
    {
        public IQueryable<T> Search();
    }
}
