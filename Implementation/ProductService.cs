using Product_CRUD_Web_API.Models;
using Product_CRUD_Web_API.Services;

namespace Product_CRUD_Web_API.Implementation
{
    public class ProductService : Service<Product>, IProductService
    {
        private ProductApiContext _context;
        public ProductService(ProductApiContext context) : base(context)
        {
            _context = context;
        }

        #region Update Product
        public void UpdateProduct(Product product)
        {
            var productFromDb = _context.Products.FirstOrDefault(u => u.ProductId == product.ProductId);

            if (productFromDb != null)
            {
                productFromDb.ProductName = product.ProductName;
                productFromDb.ProductDescription = product.ProductDescription;
                productFromDb.ProductPrice = product.ProductPrice;
            }
        }
        #endregion

        #region Update Product with PATCH
        public void UpdateProductPatch(Product product)
        {
            var productFromDb = _context.Products.FirstOrDefault(u => u.ProductId == product.ProductId);

            if (productFromDb != null)
            {
                if(productFromDb.ProductName != product.ProductName)
                {
                    productFromDb.ProductName = product.ProductName;
                }
                if(productFromDb.ProductDescription != product.ProductDescription)
                {
                    productFromDb.ProductDescription= product.ProductDescription;
                }
                if(productFromDb.ProductPrice != product.ProductPrice)
                {
                    productFromDb.ProductPrice= product.ProductPrice;
                }
            }
        }
        #endregion
    }
}
