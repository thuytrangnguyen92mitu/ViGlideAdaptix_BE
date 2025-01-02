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
		public ProductController(IProductServices productServices)
		{
			_productServices = productServices;
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
        [Authorize(Roles = "mod")]
        public async Task<IActionResult> ModCreateProduct([FromBody] CreateProductResquestDTO request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var (isCreated, message) = await _productServices.ModCreateProduct(request);
			if (isCreated)
				return Ok(message);

			return BadRequest(message);
		}
	}
}
