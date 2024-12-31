﻿using System;
using System.Collections.Generic;

namespace ViGlideAdaptix_DAL.Models;

public partial class OrderDetail
{
    public int OrderItemId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public int OrderId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

	public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
