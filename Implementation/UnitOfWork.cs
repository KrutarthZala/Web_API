using Product_CRUD_Web_API.Models;
using Product_CRUD_Web_API.Services;

namespace Product_CRUD_Web_API.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        public IProductService Product {  get; private set; }
        public IUserService User { get; private set; }

        private ProductApiContext _context;
        public UnitOfWork(ProductApiContext context)
        {
            _context = context;
            Product = new ProductService(context);
            User = new UserService(context);
        }

        public void Save()
        {
           _context.SaveChanges();
        }
    }
}
