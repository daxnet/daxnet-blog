using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Web.Models
{
    public class LoginViewModel
    {
        [StringLength(16)]
        [Display(Name = "用户名")]
        [Required(ErrorMessage = "用户名字段是必填项")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "密码字段是必填项")]
        [StringLength(16)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住本次登录")]
        public bool RememberMe { get; set; }
    }
}
