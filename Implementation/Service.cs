using Microsoft.EntityFrameworkCore;
using Product_CRUD_Web_API.Models;
using Product_CRUD_Web_API.Services;
using System.Linq.Expressions;

namespace Product_CRUD_Web_API.Implementation
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly ProductApiContext _context;
        internal DbSet<T> dbSet { get; set; }
        public Service(ProductApiContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = dbSet;
            return query.ToList();  
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }
    }
}
