using AdBoardAPI.Models;
using System.Linq;

namespace AdBoardAPI.Pagination.Searcher
{
    public interface ISearcher<T> where T : class, IModel
    {
        public IQueryable<T> Search();
    }
}
