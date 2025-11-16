using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ShopCoffee.Database;
using ShopCoffee.Helper;
using ShopCoffee.Models;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopCoffee.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CoffeeShopContext _context;
        private readonly MailHelper _mailHelper;

        public CustomerController(CoffeeShopContext context, MailHelper mailHelper)
        {
            _context = context;
            _mailHelper = mailHelper;
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
            try
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
                TempData["ProfileSuccessMessage"] = "Cập nhật thông tin thành công";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                TempData["ProfileErrorMessage"] = ex.Message;
                return RedirectToAction("Profile");
            }
        }


        // Đăng xuất
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


        #region SIGN_UP
        // ------------------ SIGN UP --------------------
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(Customer customerSignUp, IFormFile ImgUpload)
        {
            try
            {
                //Kiểm tra Email đã được đăng ký tài khoản hay chưa
                Customer? customerExist = await _context.Customers.FirstOrDefaultAsync(p => p.Email == customerSignUp.Email);

                if (customerExist != null)
                {
                    TempData["SignUpErrorMessage"] = "Email đã được đăng ký cho tài khoản khác";
                    return View();
                }

                // RegisterAt va UpdateAt được lấy tự động theo giờ hệ thống
                DateTime now = DateTime.Now;

                customerSignUp.RegisteredAt = now;
                customerSignUp.UpdateAt = now;

                //Nếu có hình ảnh được Upload
                if (ImgUpload != null)
                {
                    //Upload Hinh
                    var ImageName = await FileHelper.SaveImageAsync(ImgUpload, "customer");
                    customerSignUp.Img = ImageName;
                }
                else
                {
                    //Sử dụng avatar mặc định của project
                    customerSignUp.Img = "avatar-default.jpg";
                }

                //kiểm tra Address có null hay không
                if (customerSignUp.Address == null)
                {
                    customerSignUp.Address = "";
                }

                //HashPassword
                customerSignUp.RandomKey = PasswordHelper.GenerateRandomKey();
                customerSignUp.Password = customerSignUp.Password?.ToMd5Hash(customerSignUp.RandomKey);
                customerSignUp.Role = 1;
                customerSignUp.IsActive = true;


                await _context.Customers.AddAsync(customerSignUp);
                await _context.SaveChangesAsync();

                TempData["SignInSuccessMessage"] = "Đăng ký thành công";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return View();
            }
        }
        #endregion



        #region FORGOT_PASSWORD
        public IActionResult ForgotPassword()
        {
            //Random 1 chuỗi 6 số ngẫu nhiên
            string randomString = "";
            var rd = new Random();
            for (int i = 0; i < 6; i++)
            {
                randomString = randomString + rd.Next(0, 10).ToString();
            }

            // tạo Model lưu chuỗi random
            CustomerForgotPassword customer = new CustomerForgotPassword();
            customer.RandomCode = randomString;


            return View(customer);
        }

        [HttpPost]
        public IActionResult ForgotPassword(CustomerForgotPassword customerForgot)
        {
            if (customerForgot.RandomCode != customerForgot.OTP)
            {
                return View();
            }
            CustomerNewPassword customer = new CustomerNewPassword();
            customer.Email = customerForgot.Email;

            return RedirectToAction("ResetPassword", customer);
        }

        [HttpPost]
        public async Task<IActionResult> ForgotCheckEmailExist(string email, string otp)
        {
            //Nếu Email Null
            if (email == null)
            {
                return Json("Vui lòng nhập email");
            }

            //Nếu không tim thấy tài khoản nào sử dụng email đã nhập
            Customer? customerCheckMail = await _context.Customers.FirstOrDefaultAsync(p => p.Email == email);
            if (customerCheckMail == null)
            {
                return Json("Không tìm thấy Email");
            }

            //nếu tìm thấy
            //Gửi Email đã OTP

            string titleMail = "XÁC THỰC PHIÊN GIAO DỊCH ";
            string OTPHtml = otp;

            string body = _mailHelper.PopulateBody(OTPHtml);
            _mailHelper.SendHtmlFormattedEmail(email, titleMail, body);

            return Json("OK");
        }

        #endregion

        #region Reset_NewPassword

        public IActionResult ResetPassword(CustomerNewPassword customerNewPassword)
        {
            return View(customerNewPassword);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordPost(CustomerNewPassword customerNewPassword)
        {
            try
            {
                //Password và Confirm Password khác nhau
                if (customerNewPassword.NewPassWord != customerNewPassword.Confirm_NewPassWord)
                {
                    TempData["ResetPasswordErrorMessage"] = "Vui lòng mật khẩu giống nhau";
                    return RedirectToAction("ResetPassword", customerNewPassword);
                }

                //Kiểm tra Email đã được đăng ký tài khoản hay chưa
                Customer? customerExist = await _context.Customers.FirstOrDefaultAsync(p => p.Email == customerNewPassword.Email);
                if (customerExist == null)
                {
                    TempData["SignUpErrorMessage"] = "Email chưa được đăng ký";
                    return View();
                }

                //HashPassword
                customerNewPassword.RandomKey = PasswordHelper.GenerateRandomKey();
                customerNewPassword.NewPassWord = customerNewPassword.NewPassWord.ToMd5Hash(customerNewPassword.RandomKey);

                customerExist.RandomKey = customerNewPassword.RandomKey;
                customerExist.Password = customerNewPassword.NewPassWord;

                _context.Update(customerExist);
                await _context.SaveChangesAsync();

                //// Kiểm tra truy vấn SQL thành công hay không?
                //if (isSuccess)
                //{
                //    // Truy vấn Thành công
                //    Console.WriteLine("Update Password Success");
                //    if (HttpContext.User.FindFirstValue("CustomerId") == null)
                //    {
                //        TempData["SignInSuccessMessage"] = "Cấp lại mật khẩu thành công";
                //        return RedirectToAction("SignIn");
                //    }
                TempData["ProfileSuccessMessage"] = "Cấp lại mật khẩu thành công";
                return RedirectToAction("Profile");
                //}
                //else
                //{
                //    // Truy vấn Thất bại
                //    Console.WriteLine("Update Customer Fail");
                //    TempData["SignInSuccessMessage"] = "Lỗi hệ thống";
                //    return RedirectToAction("SignIn");
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                TempData["ResetPasswordErrorMessage"] = ex.Message;
                return RedirectToAction("ResetPassword");
            }
        }

        #endregion


    }
}
