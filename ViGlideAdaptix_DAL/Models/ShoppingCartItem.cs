using System;
using System.Collections.Generic;

namespace ViGlideAdaptix_DAL.Models;

public partial class ShoppingCartItem
{
    public int CartItemId { get; set; }

    public int CartId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual ShoppingCart Cart { get; set; } = null!;

	public decimal CalculateItemTotal()
	{
		return Quantity * UnitPrice;
	}
}
