using System;
using System.Collections.Generic;

namespace ViGlideAdaptix_DAL.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? ProductImage { get; set; }

    public string? ProductDescription { get; set; }

    public decimal UnitPrice { get; set; }

    public int CategoryId { get; set; }

    public int Quantity { get; set; }

    public int Purchases { get; set; }

    public int BrandId { get; set; }

    public int Status { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
