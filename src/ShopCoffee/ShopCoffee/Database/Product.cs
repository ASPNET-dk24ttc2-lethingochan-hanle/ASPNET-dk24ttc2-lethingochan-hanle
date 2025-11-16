using System;
using System.Collections.Generic;

namespace ShopCoffee.Database;

public partial class Product
{
    public int ProductId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public string? Img { get; set; }

    public long Price { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();
}
