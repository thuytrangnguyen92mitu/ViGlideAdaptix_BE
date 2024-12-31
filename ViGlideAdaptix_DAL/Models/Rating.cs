using System;
using System.Collections.Generic;

namespace ViGlideAdaptix_DAL.Models;

public partial class Rating
{
    public int RatingId { get; set; }

	public int OrderItemId { get; set; }

	public int CustomerId { get; set; }

    public int Score { get; set; }

    public string? Comment { get; set; }

    public DateTime RatingDate { get; set; }

    public int ProductId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

	public virtual OrderDetail OrderItem { get; set; } = null!;
}
