using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [StringLength(16)]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [StringLength(16)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住本次登录")]
        public bool RememberMe { get; set; }
    }
}
