using System.ComponentModel.DataAnnotations;

namespace ShopCoffee.Models
{
    public class CustomerLogin
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "*")]
        public string Email { get; set; } = null!;

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        public string Password { get; set; } = null!;
    }

    public class CustomerForgotPassword
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        public string Email { get; set; } = null!;

        public string RandomCode { get; set; } = null!;

        [Display(Name = "OTP")]
        [Required(ErrorMessage = "*")]
        [MaxLength(10, ErrorMessage = "Tối đa 10 kí tự")]
        public string OTP { get; set; } = null!;
    }

    public class CustomerNewPassword
    {
        public string Email { get; set; } = null!;
        public string RandomKey { set; get; } = null!;

        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "*")]
        [MaxLength(200, ErrorMessage = "Tối đa 200 kí tự")]
        public string NewPassWord { set; get; } = null!;

        [Display(Name = "Nhập lại mật khẩu mới")]
        [Required(ErrorMessage = "*")]
        [MaxLength(200, ErrorMessage = "Tối đa 200 kí tự")]
        public string Confirm_NewPassWord { set; get; } = null!;
    }


}
