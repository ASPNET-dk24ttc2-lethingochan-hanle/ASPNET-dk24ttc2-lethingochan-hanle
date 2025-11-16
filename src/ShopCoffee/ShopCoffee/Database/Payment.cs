using System;
using System.Collections.Generic;

namespace ShopCoffee.Database;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string? Phone { get; set; }

    public string Email { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public double? Total { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();
}
