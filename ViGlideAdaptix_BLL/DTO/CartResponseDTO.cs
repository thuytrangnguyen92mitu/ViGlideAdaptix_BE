using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViGlideAdaptix_DAL.Models;

namespace ViGlideAdaptix_BLL.DTO
{
	public class CartResponseDTO
	{
		public decimal SubTotal { get; set; }

		public decimal ShippingPrice { get; set; }

		public decimal TotalPrice { get; set; }

		public List<CartItemDTO> CartItemsList { get; set; } = new();

	}

	public class CartItemDTO
	{
		public int CartItemId { get; set; }

		public int CartId { get; set; }

		public int ProductId { get; set; }

		public int Quantity { get; set; }

		public decimal UnitPrice { get; set; }
	}
}
