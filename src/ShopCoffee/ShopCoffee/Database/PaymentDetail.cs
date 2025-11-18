using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopCoffee.Database;

public partial class PaymentDetail
{
    public int PaymentDetailId { get; set; }

    [Display(Name = "Đồ uống")]
    public int? ProductId { get; set; }

    public int? PaymentId { get; set; }

    [Display(Name = "Giá")]
    public long? Price { get; set; }

    [Display(Name = "Số lượng")]
    public int? Quantity { get; set; }

    [Display(Name = "Tổng tiền")]
    public long? Total { get; set; }

    [Display(Name = "Ngày thanh toán")]
    public DateTime? CreateAt { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual Product? Product { get; set; }
}
