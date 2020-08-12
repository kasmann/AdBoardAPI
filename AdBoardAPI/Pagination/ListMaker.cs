using AdBoardAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AdBoardAPI.Pagination
{

    public class ListMaker<T> where T : class, IModel
    {
        private DbSet<T> _dbSet;

        public ListMaker(DbSet<T> dbSet)
        {
            _dbSet = dbSet;
        }
        public IQueryable<T> MakeList()
        {
            return _dbSet.Select(ad => ad);
        }
    }
}
