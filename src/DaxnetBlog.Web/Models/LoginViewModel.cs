using System.ComponentModel.DataAnnotations;

namespace DaxnetBlog.Web.Models
{
    /// <summary>
    /// Represents the view model for login.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [StringLength(16, ErrorMessage = "用户名长度必须小于16个字符。")]
        [Display(Name = "用户名")]
        [Required(ErrorMessage = "用户名字段是必填项")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required(ErrorMessage = "密码字段是必填项")]
        [StringLength(20, ErrorMessage = "密码至少需要{2}个字符，最多不能超过{1}个字符", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the user inputted CAPTCHA code.
        /// </summary>
        [Required(ErrorMessage = "验证码字段是必填项")]
        [StringLength(10, ErrorMessage = "验证码长度必须小于10个字符。")]
        [Display(Name = "验证码", Prompt = "请输入下方的验证码")]
        public string Captcha { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="bool"/> value which indicates
        /// whether the web site should remember the current login user.
        /// </summary>
        [Display(Name = "记住本次登录")]
        public bool RememberMe { get; set; }
    }
}
