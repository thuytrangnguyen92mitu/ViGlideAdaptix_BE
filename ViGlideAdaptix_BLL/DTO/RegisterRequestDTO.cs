using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViGlideAdaptix_BLL.DTO
{
	public class RegisterRequestDTO
	{
		public string CustomerName { get; set; } = null!;

		public string Email { get; set; } = null!;

		public string Password { get; set; } = null!;

		public string ConfirmPassword { get; set; } = null!;

		public string Address { get; set; } = null!;

		public string PhoneNumber { get; set; } = null!;
	}
}
