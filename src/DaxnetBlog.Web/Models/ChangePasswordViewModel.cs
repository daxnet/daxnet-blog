using System.ComponentModel.DataAnnotations;

namespace DaxnetBlog.Web.Models
{
    public class ChangePasswordViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "旧密码字段是必填项")]
        [StringLength(20, ErrorMessage = "密码至少需要{2}个字符，最多不能超过{1}个字符", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "旧密码")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "新密码字段是必填项")]
        [StringLength(20, ErrorMessage = "密码至少需要{2}个字符，最多不能超过{1}个字符", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "确认密码字段是必填项")]
        [StringLength(20, ErrorMessage = "确认密码至少需要{2}个字符，最多不能超过{1}个字符", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("NewPassword", ErrorMessage = "输入的确认密码与密码不符")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "验证码字段是必填项")]
        [StringLength(10, ErrorMessage = "验证码长度必须小于10个字符。")]
        [Display(Name = "验证码")]
        public string Captcha { get; set; }
    }
}
