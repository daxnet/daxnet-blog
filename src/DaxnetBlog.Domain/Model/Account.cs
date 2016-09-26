using DaxnetBlog.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Domain.Model
{
    public class Account : IAggregateRoot<Guid>
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string NickName { get; set; }

        public string EmailAddress { get; set; }

    }
}
