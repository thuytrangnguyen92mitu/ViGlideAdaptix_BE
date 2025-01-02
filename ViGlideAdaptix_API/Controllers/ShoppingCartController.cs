using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ViGlideAdaptix_BLL.DTO;
using ViGlideAdaptix_BLL.Service.CartService;

namespace ViGlideAdaptix_API.Controllers
{
	[Route("api/cart")]
	[ApiController]
	public class ShoppingCartController : ControllerBase
	{
		private readonly IShoppingCartService _cartService;
		public ShoppingCartController(IShoppingCartService cartService)
		{
			_cartService = cartService;
		}

		/// <summary>
		/// Get Cart of Customer
		/// CartRequestDTO
		/// Return Cart, Cart Detail, Message
		/// </summary>
		[HttpPost("get")]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> GetCart([FromBody] CartRequestDTO request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var (foundCart, mess) = await _cartService.GetCartOfCustomer(request);
			if (foundCart == null)
				return BadRequest(mess);

			return Ok(new
			{
				Cart = foundCart,
				Message = mess
			});

		}

		/// <summary>
		/// 
		/// </summary>
		[HttpPost("add")]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> AddItemToCart([FromBody] AddToCartRequestDTO request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var (isSuccess, message) = await _cartService.AddItemToCart(request);
			if (isSuccess)
				return Ok(message);
			return BadRequest(message);
		}

		/// <summary>
		/// 
		/// </summary>
		[HttpPost("remove")]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> RemoveItemFromCart([FromBody] RemoveFromCartDTO request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var (isRemoved, message) = await _cartService.RemoveItemFromCart(request);
			if (isRemoved)
				return Ok(message);

			return BadRequest(message);

		}

		/// <summary>
		/// Clear Cart
		/// CartId
		/// Clear all items of that cart
		/// </summary>
		[HttpDelete("clear")]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> ClearCart([FromQuery] int cartId)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var (isDelete, message) = await _cartService.ClearCart(cartId);
			if (isDelete)
				return Ok(message);

			return BadRequest(message);
		}

		/// <summary>
		/// Check out Cart
		/// CheckOutRequestDTO
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("checkout")]
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> CheckOutCart([FromBody] CheckOutRequestDTO request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var (isCheckOut, message) = await _cartService.CheckOutCartToPayment(request);
			if (isCheckOut)
				return Ok(message);

			return BadRequest(message);
		}
	}
}
