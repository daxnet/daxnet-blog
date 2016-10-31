using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace DaxnetBlog.Web.Security
{
    public class PermissionKeyAuthorizationHandler : AuthorizationHandler<PermissionKeyRequirement>
    {
        private readonly UserManager<User> userManager;

        public PermissionKeyAuthorizationHandler(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionKeyRequirement requirement)
        {
            if (context?.User?.Identity == null)
            {
                // The controller actions marked with AuthorizeAttribute requires the authentication.
                // But if the identity instance is null, which means the authorization should fail.
                context.Fail();
                return;
            }

            if (!context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            var userName = context.User.Identity.Name;
            var user = await this.userManager.FindByNameAsync(userName);

            if (user == null)
            {
                context.Fail();
                return;
            }

            if (user.IsAdmin.HasValue && user.IsAdmin.Value)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
