using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Web.Middlewares
{
    /// <summary>
    /// The middleware that ensures Https.
    /// </summary>
    /// <remarks>
    /// Solutions is provided by http://stackoverflow.com/questions/29477393/redirect-to-https.
    /// </remarks>
    public class EnsureHttpsMiddleware
    {
        private readonly RequestDelegate next;

        public EnsureHttpsMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.IsHttps)
            {
                await next.Invoke(context);
            }
            else
            {
                var request = context.Request;

                // only redirect for GET requests, otherwise the browser might
                // not propagate the verb and request body correctly.

                if (!string.Equals(request.Method, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("This site requires HTTPS.");
                }
                else
                {
                    var newUrl = string.Concat(
                        "https://",
                        request.Host.ToUriComponent(),
                        request.PathBase.ToUriComponent(),
                        request.Path.ToUriComponent(),
                        request.QueryString.ToUriComponent());

                    context.Response.Redirect(newUrl);
                }
            }
        }
    }
}
