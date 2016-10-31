using DaxnetBlog.Common;
using DaxnetBlog.Web.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DaxnetBlog.Web.Middlewares
{
    public class ApiAuthenticationMiddleware
    {
        private const string PathPrefix = "/api/management";
        private readonly RequestDelegate nextInvocation;
        private readonly UserManager<User> userManager;
        private readonly Crypto c = Crypto.Create(CryptoTypes.EncTypeTripleDes);

        public ApiAuthenticationMiddleware(RequestDelegate nextInvocation,
            UserManager<User> userManager)
        {
            this.nextInvocation = nextInvocation;
            this.userManager = userManager;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments(new PathString(PathPrefix)))
            {
                if (context.Request.Headers.ContainsKey("DB-Authentication-Token"))
                {
                    var b64 = context.Request.Headers["DB-Authentication-Token"][0];
                    var plain = Encoding.UTF8.GetString(Convert.FromBase64String(b64));
                    var commaPosition = plain.IndexOf(":");
                    if (commaPosition == -1)
                    {
                        await nextInvocation.Invoke(context);
                    }

                    var userName = plain.Substring(0, plain.IndexOf(":"));
                    var passwordEncrypted = plain.Substring(plain.IndexOf(":") + 1, plain.Length - plain.IndexOf(":") - 1);
                    var password = c.Decrypt(passwordEncrypted, EnvironmentVariables.WebManagementApiEncryptionKey);
                    if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                    {
                        await nextInvocation.Invoke(context);
                    }

                    var authenticated = await userManager.CheckPasswordAsync(new User { UserName = userName }, password);
                    if (authenticated)
                    {
                        context.User = new ClaimsPrincipal(new GenericIdentity(userName));
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.User = null;
                }
            }
            await nextInvocation.Invoke(context);
        }
    }
}
