using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Web.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "用户名字段是必填项")]
        [Display(Name = "用户名")]
        [StringLength(16, ErrorMessage = "用户名需小于16个字符")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "电子邮件")]
        [StringLength(16, ErrorMessage = "电子邮件需小于256个字符")]
        public string Email { get; set; }

        [Display(Name = "昵称", Prompt = "若为空，则使用用户名作为昵称")]
        public string NickName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "密码至少需要{2}个字符，最多不能超过{1}个字符", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "输入的确认密码与密码不符")]
        public string ConfirmPassword { get; set; }
    }
}
