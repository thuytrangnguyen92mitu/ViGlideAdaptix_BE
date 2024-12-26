using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViGlideAdaptix_DAL.Models;

namespace ViGlideAdaptix_BLL.DTO
{
	public class LoginResponseDTO
	{
		public bool Success { get; set; }
		public int CustomerId { get; set; }
		public int RoleId { get; set; }
		public virtual Customer? Customer { get; set; }
	}
}
