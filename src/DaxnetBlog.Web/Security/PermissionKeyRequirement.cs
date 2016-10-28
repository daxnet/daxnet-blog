using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Web.Security
{
    public class PermissionKeyRequirement : IAuthorizationRequirement
    {
        public PermissionKeyRequirement(string permissionKey)
        {
            this.PermissionKey = permissionKey;
        }

        public string PermissionKey { get; set; }
    }
}
