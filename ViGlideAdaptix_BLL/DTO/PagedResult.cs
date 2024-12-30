using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViGlideAdaptix_BLL.DTO
{
	public class PagedResult<T>
	{
		public IEnumerable<T> Data { get; set; } = new List<T>();
		public int TotalRecords { get; set; }
		public int PageSize { get; set; }
		public int PageNumber { get; set; }
		public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
	}
}
