using DaxnetBlog.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
                await FillResponseWithExceptionAsync(context, ex.StatusCode, ex.IncludeFullStackTraceIfError ? ex.ToString() : ex.Message);
            }
            catch (Exception ex)
            {
                await FillResponseWithExceptionAsync(context, HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        private static async Task FillResponseWithExceptionAsync(HttpContext context, HttpStatusCode httpStatusCode, string exceptionMessage)
        {
            context.Response.StatusCode = Convert.ToInt32(httpStatusCode);
            context.Response.ContentLength = Encoding.UTF8.GetBytes(exceptionMessage).Length;
            context.Response.ContentType = "text/plain; charset=utf-8";
            await context.Response.WriteAsync(exceptionMessage);
        }
    }
}
