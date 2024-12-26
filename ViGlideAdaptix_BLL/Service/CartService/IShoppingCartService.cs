using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViGlideAdaptix_BLL.DTO;
using ViGlideAdaptix_DAL.Models;

namespace ViGlideAdaptix_BLL.Service.CartService
{
	public interface IShoppingCartService
	{
		Task<(CartResponseDTO?, string)> GetCartOfCustomer(CartRequestDTO request);
		Task<(bool, string)> ClearCart(int cartId);
	}
}
