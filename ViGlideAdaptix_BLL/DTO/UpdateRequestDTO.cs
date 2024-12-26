using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViGlideAdaptix_BLL.DTO
{
	public class UpdateRequestDTO
	{
		public int CustomerId { get; set; }
		public string CustomerName { get; set; } = null!;

		public string Password { get; set; } = null!;

		public string Address { get; set; } = null!;

		public string PhoneNumber { get; set; } = null!;
	}
}
