using System.Linq.Expressions;

namespace Product_CRUD_Web_API.Services
{
    public interface IService<T> where T : class
    {
        // T - Product, T - User
        IEnumerable<T> GetAll();
        T Get(Expression<Func<T, bool>> filter);
        void Add(T entity);
        void Remove(T entity);

    }
    
}
