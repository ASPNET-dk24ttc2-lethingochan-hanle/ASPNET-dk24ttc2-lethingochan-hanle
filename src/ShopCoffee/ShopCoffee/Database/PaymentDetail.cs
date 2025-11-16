using System;
using System.Collections.Generic;

namespace ShopCoffee.Database;

public partial class PaymentDetail
{
    public int PaymentDetailId { get; set; }

    public int? ProductId { get; set; }

    public int? PaymentId { get; set; }

    public long? Price { get; set; }

    public int? Quantity { get; set; }

    public long? Total { get; set; }

    public DateTime? CreateAt { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual Product? Product { get; set; }
}
