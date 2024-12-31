using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViGlideAdaptix_BLL.DTO
{
	public class AllProductResponseDTO
	{
		public int ProductId { get; set; }

		public string ProductName { get; set; } = null!;

		public string? ProductImage { get; set; }

		public string? ProductDescription { get; set; }

		public decimal UnitPrice { get; set; }

		public int CategoryId { get; set; }

		public int Quantity { get; set; }

		public int Purchases { get; set; }

		public int BrandId { get; set; }

		public double RatingScore { get; set; }

	}
}
