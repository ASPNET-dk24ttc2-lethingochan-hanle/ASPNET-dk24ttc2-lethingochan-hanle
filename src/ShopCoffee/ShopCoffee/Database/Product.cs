using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopCoffee.Database;

public partial class Product
{
    
    public int ProductId { get; set; }

    [Display(Name = "Tên món")]
    public string Title { get; set; } = null!;

    [Display(Name = "Mô tả")]
    public string? Content { get; set; }

    [Display(Name = "Ảnh")]
    public string? Img { get; set; }

    [Display(Name = "Giá")]
    public long Price { get; set; }

    [Display(Name = "Ngày thêm")]
    public DateTime? CreateAt { get; set; }

    [Display(Name = "Lần chỉnh sửa gần đây")]
    public DateTime? UpdateAt { get; set; }

    [Display(Name = "Danh mục")]
    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();
}
