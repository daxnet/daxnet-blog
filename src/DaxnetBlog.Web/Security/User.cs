using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Web.Security
{
    public class User
    {
        public string UserName { get; set; }

        public string NickName { get; set; }

        public int Id { get; set; }

        public string EmailAddress { get; set; }

        public bool? IsLocked { get; set; }

        public override string ToString() => UserName;
    }
}
