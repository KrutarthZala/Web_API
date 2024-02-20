using Product_CRUD_Web_API.Models;
using Product_CRUD_Web_API.Services;

namespace Product_CRUD_Web_API.Implementation
{
    public class UserService : Service<UserModel>, IUserService
    {       
        private ProductApiContext _context;
        public UserService(ProductApiContext context) : base(context)
        {
            _context = context;
        }
    }
    
}
