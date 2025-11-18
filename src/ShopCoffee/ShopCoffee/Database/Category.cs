using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopCoffee.Database;

public partial class Category
{

    public int CategoryId { get; set; }

    [Display(Name = "Tên danh mục")]
    public string Title { get; set; } = null!;

    [Display(Name = "Mô tả")]
    public string? Content { get; set; }

    [Display(Name = "Ngày thêm")]
    public DateTime CreateAt { get; set; }

    [Display(Name = "Lần chỉnh sửa gần đây")]
    public DateTime? UpdateAt { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
