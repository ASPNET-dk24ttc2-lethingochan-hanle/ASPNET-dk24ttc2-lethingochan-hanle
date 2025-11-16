using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCoffee.Database;
using ShopCoffee.Helper;
using ShopCoffee.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopCoffee.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CoffeeShopContext _context;

        public CustomerController(CoffeeShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int idCustomer = HttpContext.Session?.GetInt32("CustomerId") ?? 0;
            if (idCustomer == 0) return RedirectToAction("Login", new { ReturnUrl = "/Customer/Profile" });

            var customer = await _context.Customers
                    .Include(p => p.Payments)
                        .ThenInclude(p => p.PaymentDetails)
                        .ThenInclude(p => p.Product)
                    .FirstOrDefaultAsync(p => p.CustomerId == idCustomer);

            return View(customer);
        }

        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }


        // POST: Customer/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(CustomerLogin model, string? ReturnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            ViewBag.ReturnUrl = ReturnUrl;

            // Kiểm tra email + mật khẩu
            var customer = _context.Customers
                .FirstOrDefault(x => x.Email == model.Email);

            if (customer == null)
            {
                TempData["ErrorMessage"] = "Email hoặc mật khẩu không đúng!";
                return View(model);
            }
            if (model.Password == null)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập mật khẩu");
                return View(model);
            }

            string hashPassword = model.Password.ToMd5Hash(customer.RandomKey);
            if (hashPassword != customer.Password)
            {
                TempData["ErrorMessage"] = "Mật khẩu không đúng!";
                return View(model);
            }

            if (!customer.IsActive)
            {
                TempData["ErrorMessage"] = "Tài khoản của bạn đang bị khóa!";
                return View(model);
            }

            // Lưu session
            HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
            HttpContext.Session.SetString("CustomerName", customer.FirstName + " " + (customer.LastName ?? ""));
            HttpContext.Session.SetString("Role", customer.Role == 0 ? "Administrator" : "Customer");

            TempData["SignInSuccessMessage"] = "Đăng nhập thành công";

            if (Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            else
            {
                //if (customer.Role == 0)
                //    return RedirectToAction("Index", "Admin"); // nếu là admin
                //else
                return RedirectToAction("Index", "Product"); // nếu là user
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDetailCustomer(Customer model, IFormFile? ImgUpload)
        {
            if (ImgUpload != null)
            {
                model.Img = await FileHelper.SaveImageAsync(ImgUpload, "customer");
            }
            else
            {
                model.Img ??= Url.Content("~/images/placeholder.png");
            }

            model.UpdateAt = DateTime.Now;
            _context.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Profile));
        }


        // Đăng xuất
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
