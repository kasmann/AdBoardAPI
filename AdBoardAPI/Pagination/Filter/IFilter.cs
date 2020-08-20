using AdBoardAPI.Models;
using System.Linq;

namespace AdBoardAPI.Pagination.Filter
{
    public interface IFilter<T> where T : IModel
    {
        public IQueryable<T> Filter();
    }
}
