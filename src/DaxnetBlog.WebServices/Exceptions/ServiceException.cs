using DaxnetBlog.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Collections;

namespace DaxnetBlog.WebServices.Exceptions
{
    public class ServiceException : DaxnetBlogException
    {
        public ServiceException(Reason reason, string message)
            : this(HttpStatusCode.InternalServerError, reason, message)
        { }

        public ServiceException(HttpStatusCode statusCode, Reason reason, string message)
            : base(message)
        {
            this.StatusCode = statusCode;
            this.Reason = reason;
        }

        public HttpStatusCode StatusCode { get; }

        public Reason Reason { get; }
    }
}
