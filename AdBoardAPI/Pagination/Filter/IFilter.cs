using AdBoardAPI.Models;
using System.Linq;

namespace AdBoardAPI.Pagination.Filter
{
    public interface IFilter<T> where T : class, IModel
    {
        public IQueryable<T> Filter();
    }
}
