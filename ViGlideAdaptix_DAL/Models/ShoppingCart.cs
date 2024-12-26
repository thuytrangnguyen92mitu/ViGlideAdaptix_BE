using System;
using System.Collections.Generic;

namespace ViGlideAdaptix_DAL.Models;

public partial class ShoppingCart
{
    public int CartId { get; set; }

    public DateTime CreatedDate { get; set; }

    public decimal SubTotal { get; set; }

    public decimal ShippingPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public int CustomerId { get; set; }

    public bool IsActive { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
}
