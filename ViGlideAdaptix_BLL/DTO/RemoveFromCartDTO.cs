﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViGlideAdaptix_BLL.DTO
{
	public class RemoveFromCartDTO
	{
		public int CartId { get; set; }
		public int CartItemId { get; set; }
		public int ProductId { get; set; }

	}
}
