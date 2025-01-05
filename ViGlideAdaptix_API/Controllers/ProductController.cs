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
            //get path to the image requested
            var imagePath = Path.Combine(_env.ContentRootPath, "uploads", imageName);

            if (!System.IO.File.Exists(imagePath))
            {
                return NoContent();
            }

            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                using (var defaultUserImg = new FileStream(imagePath, FileMode.Open))
                {
                    defaultUserImg.CopyTo(ms);
                }
                fileBytes = ms.ToArray();
            }

            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(base64String);
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

        /// <summary>
        /// Get all brand list
        /// </summary>
        [HttpGet("brand")]
        public async Task<IActionResult> GetAllBrand()
        {
            var result = await _productServices.GetBrandList();
            return Ok(result);
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
        /// 
        /// </summary>
        [HttpPost("add")]
        //[Authorize(Roles = "mod")]
        public async Task<IActionResult> ModCreateProduct([FromForm] CreateProductResquestDTO request, [FromForm] IFormFile? productImage)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (productImage == null || productImage.Length == 0)
            {
                return BadRequest("No image uploaded.");
            }
            var uploadPath = Path.Combine(_env.ContentRootPath, "uploads");
            Directory.CreateDirectory(uploadPath); // Ensure the directory exists

           //path to the location to save image (with the file name included) .ie : <solution>/uploads/<filename>.<ext>
            var filePath = Path.Combine(uploadPath, Path.GetFileName(productImage.FileName));

            // Save the image to the server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await productImage.CopyToAsync(stream);
            }

            //Generate image path and create Product
            request.ProductImage = productImage.FileName;

            var (isCreated, message) = await _productServices.ModCreateProduct(request);
            if (isCreated)
                return Ok(message);

            return BadRequest(message);
        }
    }
}
