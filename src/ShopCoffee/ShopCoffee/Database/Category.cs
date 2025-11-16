using System;
using System.Collections.Generic;

namespace ShopCoffee.Database;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
