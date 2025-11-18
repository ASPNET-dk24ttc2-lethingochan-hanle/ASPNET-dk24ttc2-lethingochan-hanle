using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopCoffee.Database;

public partial class Payment
{
    public int PaymentId { get; set; }

    [Display(Name = "Khách hành")]
    public int CustomerId { get; set; }

    [Display(Name = "Tên")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Họ")]
    public string? LastName { get; set; }

    [Display(Name = "SĐT")]
    public string? Phone { get; set; }

    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Display(Name = "Ngày thanh toán")]
    public DateTime CreateAt { get; set; }

    [Display(Name = "Tổng tiền")]
    public double? Total { get; set; }

    [Display(Name = "Khách hàng")]
    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();
}
