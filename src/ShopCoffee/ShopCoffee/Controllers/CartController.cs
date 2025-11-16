using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ShopCoffee.Database;
using ShopCoffee.Helper;
using ShopCoffee.Models;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopCoffee.Controllers
{
    public class CartController : Controller
    {
        private readonly CoffeeShopContext _context;
        public CartController(CoffeeShopContext context)
        {
            _context = context;
        }

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MyConst.CART_KEY) ??
            new List<CartItem>();

        public IActionResult Index()
        {
            return View(Cart);
        }
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.IdProduct == id);
            if (item == null)
            {
                Product? productById = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);

                if (productById == null)
                {
                    TempData["Message"] = "Khong tim thay san pham";
                    return Redirect("/404");
                }

                item = new CartItem
                {
                    IdProduct = productById.ProductId,
                    Img = productById.Img ?? "",
                    Name = productById.Title,
                    Price = productById.Price,
                    Quantity = quantity
                };
                gioHang.Add(item);
            }
            else
            {
                item.Quantity += quantity;
            }

            HttpContext.Session.Set(MyConst.CART_KEY, gioHang);

            return RedirectToAction("Index");
        }
        public IActionResult ChangeQuantityCart(int id, bool isIncrement = true, int quantity = 1)
        {
            // Lấy toàn bộ giỏ hàng 
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.IdProduct == id);

            //kiểm tra tồn tại Product 
            if (item == null)
            {

                TempData["Message"] = "Khong tim thay san pham";
                return Redirect("/404");
            }
            else
            {
                // Nếu là button tăng số lượng 
                if (isIncrement)
                {
                    item.Quantity += quantity;
                }
                // Nếu là button giảm số lượng 
                else
                {
                    item.Quantity -= quantity;
                    // Nếu khách hàng nhập số lượng <= 0 thì xóa sản phẩm đó ra khỏi giỏ
                    if (item.Quantity <= 0)
                    {
                        gioHang.Remove(item);
                    }
                }
            }

            // Lưu thay đổi 
            HttpContext.Session.Set(MyConst.CART_KEY, gioHang);

            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;

            var item = gioHang.SingleOrDefault(p => p.IdProduct == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MyConst.CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CheckOut()
        {
            try
            {
                int idCustomer = HttpContext.Session?.GetInt32("CustomerId") ?? 0;

                if (idCustomer == 0)
                {
                    return Redirect("/Customer/Login");
                }
                //Không thanh toán khi không có sản phẩm trong giỏ hàng 
                if (Cart.Count == 0)
                {
                    TempData["CheckOutErrorMessage"] = "Thanh toán thất bại";
                    return RedirectToAction("Index");
                }
                // Lấy Customer id nếu người dùng đã đăng nhập 


                Customer? customer = new();
                customer = await _context.Customers.FirstOrDefaultAsync(p => p.CustomerId == idCustomer);

                // Nếu không tìm thấy User thì trả về trang 404 - Not Found 
                if (customer == null)
                {
                    return Redirect("/404");
                }

                //Insert Vào Database 
                var paymentAdd = new Payment()
                {
                    CustomerId = customer.CustomerId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Phone = customer.Phone,
                    Email = customer.Email,
                    CreateAt = DateTime.Now,
                    Total = Cart.Sum(p => p.Total)
                };
                await _context.Payments.AddAsync(paymentAdd);
                await _context.SaveChangesAsync();

                var listAdd = new List<PaymentDetail>();
                foreach (var item in Cart)
                {
                    var itemAdd = new PaymentDetail()
                    {
                        ProductId = item.IdProduct,
                        PaymentId = paymentAdd.PaymentId,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        Total = item.Total,
                        CreateAt = DateTime.Now,
                    };
                    listAdd.Add(itemAdd);
                }
                await _context.PaymentDetails.AddRangeAsync(listAdd);
                await _context.SaveChangesAsync();

                TempData["CheckOutSuccessMessage"] = "Thanh toán thành công";

                var gioHang = Cart;
                gioHang = new List<CartItem>();
                HttpContext.Session?.Set(MyConst.CART_KEY, gioHang);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                TempData["CheckOutErrorMessage"] = "Thanh toán thất bại: " + ex.Message.ToString();
                return RedirectToAction("Index");
            }

        }
    }
}
