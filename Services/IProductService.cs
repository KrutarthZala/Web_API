using Product_CRUD_Web_API.Models;

namespace Product_CRUD_Web_API.Services
{
    public interface IProductService : IService<Product>
    {
        void UpdateProduct(Product product);
        void UpdateProductPatch(Product product);
    }
}
