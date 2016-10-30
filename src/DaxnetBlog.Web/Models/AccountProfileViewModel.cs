using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Web.Models
{
    /// <summary>
    /// The view model used for displaying and managing account profile.
    /// </summary>
    public class AccountProfileViewModel
    {
        [Display(Name = "ID")]
        [DataType(DataType.Text)]
        public int Id { get; set; }

        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Display(Name = "昵称", Prompt = "若为空，则使用用户名作为昵称")]
        public string NickName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "电子邮件")]
        [StringLength(256, ErrorMessage = "电子邮件需小于256个字符")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "验证码字段是必填项")]
        [StringLength(10)]
        [Display(Name = "验证码")]
        public string Captcha { get; set; }
    }
}
