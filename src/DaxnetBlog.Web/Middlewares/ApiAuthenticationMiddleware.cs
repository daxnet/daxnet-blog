using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Web.Middlewares
{
    public class ApiAuthenticationMiddleware
    {
        private readonly RequestDelegate nextInvocation;

        public ApiAuthenticationMiddleware(RequestDelegate nextInvocation)
        {
            this.nextInvocation = nextInvocation;
        }

        public async Task Invoke(HttpContext context)
        {
            if (this.nextInvocation.Target != null)
            {

            }
            await this.nextInvocation.Invoke(context);
        }
    }
}
