using System;
using System.Collections.Generic;

namespace ShopCoffee.Database;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string Email { get; set; } = null!;

    public string? Img { get; set; }

    public DateTime RegisteredAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Password { get; set; }

    public string? RandomKey { get; set; }

    public bool IsActive { get; set; }

    public int Role { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
