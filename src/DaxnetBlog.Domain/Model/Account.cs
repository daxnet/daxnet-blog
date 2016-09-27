using DaxnetBlog.Common;
using System;

namespace DaxnetBlog.Domain.Model
{
    public class Account : IEntity<int>
    {

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string NickName { get; set; }

        public string EmailAddress { get; set; }

        public DateTime DateRegistered { get; set; }

        public DateTime? DateLastLogin { get; set; }

        public int Id { get; set; }
    }
}
