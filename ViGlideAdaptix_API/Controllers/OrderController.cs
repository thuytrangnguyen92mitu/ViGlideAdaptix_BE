using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO.IsolatedStorage;
using ViGlideAdaptix_BLL.DTO;
using ViGlideAdaptix_BLL.Service.OrderService;

namespace ViGlideAdaptix_API.Controllers
{
	[Route("api/order")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly IOrderService _orderService;
		public OrderController(IOrderService orderService)
		{
			_orderService = orderService;
		}

		[HttpPost("confirm/{orderId}")]
        public async Task<IActionResult> ConfirmOrder(int orderId)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var (isConfirm, message) = await _orderService.ConfirmOrder(orderId);
			if (isConfirm)
				return Ok(message);

			return BadRequest(message);
		}

		[HttpPost("cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var (isCancel, message) = await _orderService.CancelOrder(orderId);
			if (isCancel)
				return Ok(message);

			return BadRequest(message);
		}

		[HttpPost("rating")]
        public async Task<IActionResult> RatingProduct([FromQuery] int orderId, [FromBody] RatingRequestDTO request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
	
			var (isSuccess, message) = await _orderService.RatingProduct(orderId, request);
			if(isSuccess)
				return Ok(message);

			return BadRequest(message);

		}
	}
}
