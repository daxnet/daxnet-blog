using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.WebServices.Middlewares
{
    public class CustomServiceResponseTimeMiddleware
    {
        private readonly RequestDelegate nextInvocation;
        private readonly ILoggerFactory loggerFactory;

        public CustomServiceResponseTimeMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            nextInvocation = next;
            this.loggerFactory = loggerFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopWatch = new Stopwatch();

            context.Response.OnStarting((sw) =>
            {
                var localWatch = (Stopwatch)sw;
                localWatch.Stop();
                context.Response.Headers.Add("X-Processing-Time", new[] { localWatch.Elapsed.ToString() });
                return Task.CompletedTask;
            }, stopWatch);

            stopWatch.Start();
            await nextInvocation.Invoke(context);
        }
    }
}
