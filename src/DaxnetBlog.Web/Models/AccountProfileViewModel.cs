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

        public string NickName { get; set; }

        public string EmailAddress { get; set; }
    }
}
