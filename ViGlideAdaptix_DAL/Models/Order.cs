using System;
using System.Collections.Generic;

namespace ViGlideAdaptix_DAL.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int CartId { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CustomerId { get; set; }

    public decimal TotalPrice { get; set; }

    public int PaymentMethodId { get; set; }

    public DateTime EstDeliveryDate { get; set; }

    public DateTime? RealDeliveryDate { get; set; }

    public double? Tax { get; set; }

    public int Status { get; set; }

    public virtual ShoppingCart Cart { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

}
