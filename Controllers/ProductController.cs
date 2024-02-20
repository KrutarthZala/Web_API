using Microsoft.AspNetCore.Mvc;
using Product_CRUD_Web_API.Models;
using Product_CRUD_Web_API.Services;

namespace Product_CRUD_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    { 
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        #region HTTP GET Method
        [HttpGet]
        public IActionResult GetProducts()
        {
            //IEnumerable < Product >
            //return await _context.Products.ToListAsync();
            try
            {
                var products =  _unitOfWork.Product.GetAll();

                // Validate products
                if (products == null || !products.Any())
                {
                    return NotFound(new 
                    {
                        StatusCode = 404,
                        Message = "No products found"
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Success",
                    Data = products
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Details = ex.Message });
            }
        }
        #endregion

        #region HTTP GET with Specific Product ID
        [HttpGet("{ProductId}")]
        public IActionResult GetSpecificProduct(int ProductId)
        {
            // Validate ProductId
            if (ProductId < 1)
            {
                return BadRequest(new {StatusCode=400, Message="Bad Request"});
            }

            var product =  _unitOfWork.Product.Get(p => p.ProductId == ProductId);

            // Validate product
            if (product == null)
            {
                return NotFound(new {StatusCode=404, Message="Not Found"});
            }
            return Ok(new { StatusCode = 200, Message="Success" , Data = product});
        }
        #endregion

        #region HTTP POST Method / Insert Product
        [HttpPost]
        public IActionResult PostProduct(Product p)
        {
            try
            {
                // Validate product
                if (p == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Bad Request"
                    });
                }

                _unitOfWork.Product.Add(p);
                _unitOfWork.Save();
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Product inserted successfully."
                });
            }
            catch(Exception ex)
            {
                return StatusCode(406, new { Message = "Not Acceptable", Details = ex.Message });
            }
        }
        #endregion

        #region HTTP PUT Method / Update the Product
        [HttpPut]
        public IActionResult PutProduct(Product productData)
        {
            // Validate ProductId and productData
            if( productData == null || productData.ProductId == 0)
            {
                return BadRequest(new {StatusCode=400, Message= "Bad Request"});
            }

            var product = _unitOfWork.Product.Get(p => p.ProductId == productData.ProductId);

            // Validate product
            if (product == null)
            {
                return NotFound(new {StatusCode=404, Message="Not Found"});
            }

            _unitOfWork.Product.UpdateProduct(productData);
            _unitOfWork.Save();
            return Ok(new {StatusCode=200, Message="Product Updated successfully"});
        }
        #endregion

        #region HTTP PATCH Method 
        [HttpPatch]
        public IActionResult PatchProduct(Product productData)
        {
            // Validate ProductId and productData
            if (productData == null || productData.ProductId == 0)
            {
                return BadRequest(new { StatusCode = 400, Message = "Bad Request" });
            }

            var product = _unitOfWork.Product.Get(p => p.ProductId == productData.ProductId);

            // Validate product
            if (product == null)
            {
                return NotFound(new { StatusCode = 404, Message = "Not Found" });
            }

            _unitOfWork.Product.UpdateProductPatch(productData);
            _unitOfWork.Save();
            return Ok(new { StatusCode = 200, Message = "Product Updated successfully with HTTP PATCH " });

        }
        #endregion

        #region HTTP DELETE Method
        [HttpDelete("{productID}")]
        public IActionResult DeleteProduct(int productID)
        {
            // Validate productID
            if( productID < 1) 
            {
                return BadRequest(new { StatusCode = 400, Message = "Bad Request" }); 
            }

            var product = _unitOfWork.Product.Get(p => p.ProductId == productID);

            // Validate product
            if(product == null)
            {
                return NotFound(new { StatusCode = 404, Message = "Not Found" });
            }
            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();
            return Ok(new { StatusCode = 200, Message = "Product deleted successfully" });
        }
        #endregion

        #region Upload Image in Folder
        [HttpPost("uploadInFolder")]
        public async Task<IActionResult> UploadProductImageInFolder(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var folderPath = _webHostEnvironment.WebRootPath;
                var imgPath = Path.Combine(folderPath, @"images");

                // Check if the directory exists, if not, create it
                if (!Directory.Exists(imgPath))
                {
                    Directory.CreateDirectory(imgPath);
                }

                var filePath = Path.Combine(imgPath, file.FileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
               
                return Ok(new { StatusCode=200 , Message="File uploaded successfully" });
            }
            else
            {
                return BadRequest(new {StatusCode=400, Message = "No file uploaded." });
            }
        }
        #endregion

        #region Upload Product Image in Product Table
        [HttpPost("upload")]
        public IActionResult UploadProductImageInDatabase(int productID, IFormFile file)
        {
            // Validate product ID
            var product = _unitOfWork.Product.Get(p => p.ProductId == productID);
            if (product == null)
            {
                return NotFound(new {StatusCode=404, Message = "Product not found" });
            }

            // Validate file
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { StatusCode=400, Message = "No file uploaded" });
            }

            #region Convert File into Base64 String
            // Read the file content into a byte array
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            // Convert the byte array to base64 string
            string base64Content = Convert.ToBase64String(fileBytes);
            #endregion

            // Set Image Path and Image Size
            product.ProductImagePath = file.FileName;
            //product.ProductImageSize = Convert.ToString(file.Length);

            #region Define File Size
            // Convert file size byte into KB or MB
            double lengthInKB = Convert.ToDouble(file.Length / 1024);
            if (lengthInKB < 1024)
            {
                product.ProductImageSize = Convert.ToString(lengthInKB) +" KB";
            }
            else
            {
                product.ProductImageSize = Convert.ToString(lengthInKB / 1024) + " MB";
            }
            #endregion

            // Save the file content to the ProductImage column
            product.ProductImage = base64Content;
            _unitOfWork.Save();

            return Ok(new { StatusCode=200, Message = "File content saved successfully in database." });
        }
        #endregion
        
    }
}
