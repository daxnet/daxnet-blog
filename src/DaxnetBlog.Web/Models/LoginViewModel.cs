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
        public string UserName { get; set; }

        [Required]
        [StringLength(16)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
