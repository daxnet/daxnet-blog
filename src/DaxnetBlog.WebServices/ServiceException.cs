using DaxnetBlog.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DaxnetBlog.WebServices
{
    public class ServiceException : DaxnetBlogException
    {

        public ServiceException()
            : this(HttpStatusCode.InternalServerError)
        { }

        public ServiceException(HttpStatusCode statusCode) : base()
        { this.StatusCode = statusCode; }

        public ServiceException(string message)
            : this(HttpStatusCode.InternalServerError, message)
        { }

        public ServiceException(HttpStatusCode statusCode, string message)
            : base(message)
        { this.StatusCode = statusCode; }

        public ServiceException(string format, params object[] args)
            : this(HttpStatusCode.InternalServerError, string.Format(format, args))
        {  }

        public ServiceException(HttpStatusCode statusCode, string format, params object[] args)
            : base(string.Format(format, args))
        { this.StatusCode = statusCode; }

        public ServiceException(string message, Exception innerException)
            : this(HttpStatusCode.InternalServerError, message, innerException)
        { }

        public ServiceException(HttpStatusCode statusCode, string message, Exception innerException)
            : base(message, innerException)
        { this.StatusCode = statusCode; }

        public HttpStatusCode StatusCode { get; }
    }
}
