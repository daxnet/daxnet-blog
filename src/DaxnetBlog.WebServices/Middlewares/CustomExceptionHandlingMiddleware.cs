using DaxnetBlog.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DaxnetBlog.WebServices.Middlewares
{
    public class CustomExceptionHandlingMiddleware
    {
        private readonly RequestDelegate nextInvocation;
        private readonly ILoggerFactory loggerFactory;

        public CustomExceptionHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            nextInvocation = next;
            this.loggerFactory = loggerFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await nextInvocation.Invoke(context);
            }
            catch (DomainException ex)
            {
                await FillResponseWithExceptionAsync(context, HttpStatusCode.InternalServerError, ex.ToString());
            }
            catch (ServiceException ex)
            {
                await FillResponseWithExceptionAsync(context, ex.StatusCode, ex.ToString());
            }
            catch (Exception ex)
            {
                await FillResponseWithExceptionAsync(context, HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        private static async Task FillResponseWithExceptionAsync(HttpContext context, HttpStatusCode httpStatusCode, string exceptionMessage)
        {
            context.Response.StatusCode = Convert.ToInt32(httpStatusCode);
            context.Response.ContentLength = exceptionMessage.Length;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(exceptionMessage);
        }
    }
}
