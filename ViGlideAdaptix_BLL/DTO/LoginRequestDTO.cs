using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViGlideAdaptix_BLL.DTO
{
	public class LoginRequestDTO
	{
		public string Email { get; set; } = null!;

		public string Password { get; set; } = null!;
	}
}
