using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViGlideAdaptix_BLL.DTO
{
	public class QueryObjectDTO
	{
		public string? ProductName { get; set; } = null;
		public int CategoryId { get; set; }
		public int BrandId { get; set; }
		public string? SortBy { get; set; } = null;
		public bool IsDecsending { get; set; } = false;
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 16;
	}
}
