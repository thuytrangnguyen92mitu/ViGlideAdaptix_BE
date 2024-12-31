using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViGlideAdaptix_BLL.DTO
{
	public class RatingRequestDTO
	{
		public int OrderItemId { get; set; }

		public int Score { get; set; }

		public string? Comment { get; set; }

		public DateTime RatingDate { get; set; }
	}
}
