using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViGlideAdaptix_BLL.DTO;

namespace ViGlideAdaptix_BLL.Service.OrderService
{
	public interface IOrderService
	{
		Task<(bool, string)> ConfirmOrder(int orderId);
		Task<(bool, string)> CancelOrder(int orderId);
		Task<(bool, string)> RatingProduct(int orderId, RatingRequestDTO request);
	}
}
