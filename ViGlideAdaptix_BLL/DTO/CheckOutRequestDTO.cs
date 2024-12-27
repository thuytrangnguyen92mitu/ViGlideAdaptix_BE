using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViGlideAdaptix_BLL.DTO
{
	public class CheckOutRequestDTO
	{
		public int CartId { get; set; }
		public int CustomerId { get; set; }
		public int PaymentMethodId { get; set; }
	}
}
