using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ViGlideAdaptix_BLL.DTO;
using ViGlideAdaptix_BLL.Service.ProductService;

namespace ViGlideAdaptix_API.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productServices;
        private readonly IWebHostEnvironment _env;
        public ProductController(IProductServices productServices, IWebHostEnvironment env)
        {
            _productServices = productServices;
            _env = env;
        }

        /// <summary>
        /// Get all product
        /// QueryObjectDTO
        /// Return list of data + total Record 
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllProduct([FromQuery] QueryObjectDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productServices.GetAllProductAsync(request);
            return Ok(result);
        }

        [HttpGet("images")]
        public IActionResult GetImages(string imageName)
        {
            var imagePath = Path.Combine(_env.ContentRootPath, "uploads", imageName);

            if (!System.IO.File.Exists(imagePath))
            {
                return NoContent();
            }

            byte[] fileBytes;
            string mimeType;

            // Detect MIME type based on file extension
            string fileExtension = Path.GetExtension(imageName).ToLower();
            switch (fileExtension)
            {
                case ".jpeg":
                case ".jpg":
                    mimeType = "image/jpeg";
                    break;
                case ".png":
                    mimeType = "image/png";
                    break;
                case ".gif":
                    mimeType = "image/gif";
                    break;
                case ".webp":
                    mimeType = "image/webp";
                    break;
                default:
                    mimeType = "application/octet-stream"; // fallback to binary if unrecognized
                    break;
            }

            // Read image data
            using (var ms = new MemoryStream())
            {
                using (var defaultUserImg = new FileStream(imagePath, FileMode.Open))
                {
                    defaultUserImg.CopyTo(ms);
                }
                fileBytes = ms.ToArray();
            }

            string base64String = Convert.ToBase64String(fileBytes);

            // Return the image in base64 format
            return Ok(new { image = $"data:{mimeType};base64,{base64String}" });
        }

        [HttpGet("newArrival")]
        public async Task<IActionResult> GetNewArrivalProduct()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productServices.GetNewArrivalProducts();
            return Ok(result);
        }

        [HttpGet("bestSeller")]
        public async Task<IActionResult> GetBestSellerProduct()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productServices.GetBestSellerProducts();
            return Ok(result);
        }

        /// <summary>
        /// Get all category list
        /// </summary>
        [HttpGet("category")]
        public async Task<IActionResult> GetAllCategory()
        {
            var result = await _productServices.GetCateList();
            return Ok(result);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            var cate = await _productServices.GetCateName(categoryId);
            return Ok(cate);
        }

        /// <summary>
        /// Get all brand list
        /// </summary>
        [HttpGet("brand")]
        public async Task<IActionResult> GetAllBrand()
        {
            var result = await _productServices.GetBrandList();
            return Ok(result);
        }
        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetBrandById(int brandId)
        {
            var brand = await _productServices.GetBrandName(brandId);
            return Ok(brand);
        }
        /// <summary>
        /// Get detail of product
        /// productId
        /// </summary>
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetDetailOfProduct(int productId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var foundProduct = await _productServices.GetProductDetail(productId);
            return Ok(foundProduct);
        }

        /// <summary>
        /// Mod create new product
        /// CreateProductResquestDTO
        /// productImage
        /// </summary>
        [HttpPost("add")]
        //[Authorize(Roles = "mod")]
        public async Task<IActionResult> ModCreateProduct([FromForm] CreateProductResquestDTO request, [FromForm] IFormFile? productImage)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (productImage == null || productImage.Length == 0)
                return BadRequest("No image uploaded.");

            // Define the upload path
            var uploadPath = Path.Combine(_env.ContentRootPath, "uploads");

            // Ensure the directory exists
            Directory.CreateDirectory(uploadPath);

            // Generate a unique file name based on GUID
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(productImage.FileName)}";

            // Define the full file path to store the image
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            // Save the image to the uploads directory
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await productImage.CopyToAsync(stream);
            }

            // Store only the image file name (not the full path) in the request DTO
            request.ProductImage = uniqueFileName;

            // Call the service to create the product
            var (isCreated, message) = await _productServices.ModCreateProduct(request);

            // Return the response
            return isCreated ? Ok(message) : BadRequest(message);
        }


    }
}
