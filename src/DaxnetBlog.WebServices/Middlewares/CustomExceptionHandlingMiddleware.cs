using DaxnetBlog.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DaxnetBlog.WebServices.Exceptions;
using Newtonsoft.Json;

namespace DaxnetBlog.WebServices.Middlewares
{
    public class CustomExceptionHandlingMiddleware
    {
        private readonly RequestDelegate nextInvocation;
        private readonly ILogger<CustomExceptionHandlingMiddleware> logger;

        public CustomExceptionHandlingMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlingMiddleware> logger)
        {
            nextInvocation = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await nextInvocation.Invoke(context);
            }
            catch (DomainException ex)
            {
                logger.LogWarning(ex.ToString());
                await FillResponseWithExceptionAsync(context, HttpStatusCode.InternalServerError, ex.ToString());
            }
            catch (ServiceException ex)
            {
                logger.LogWarning(ex.ToString());
                await FillResponseWithExceptionAsync(context, ex.StatusCode, JsonConvert.SerializeObject(new
                {
                    Reason = ex.Reason,
                    Message = ex.Message
                }));
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex.ToString());
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
