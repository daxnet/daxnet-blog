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

        public ServiceException(bool includeFullStackTraceIfError = true)
            : this(HttpStatusCode.InternalServerError, includeFullStackTraceIfError)
        { }

        public ServiceException(HttpStatusCode statusCode, bool includeFullStackTraceIfError = true) : base()
        {
            this.StatusCode = statusCode;
            this.IncludeFullStackTraceIfError = includeFullStackTraceIfError;
        }

        public ServiceException(string message, bool includeFullStackTraceIfError = true)
            : this(HttpStatusCode.InternalServerError, message, includeFullStackTraceIfError)
        { }

        public ServiceException(HttpStatusCode statusCode, string message, bool includeFullStackTraceIfError = true)
            : base(message)
        {
            this.StatusCode = statusCode;
            this.IncludeFullStackTraceIfError = includeFullStackTraceIfError;
        }

        public ServiceException(string message, Exception innerException, bool includeFullStackTraceIfError = true)
            : this(HttpStatusCode.InternalServerError, message, innerException, includeFullStackTraceIfError)
        { }

        public ServiceException(HttpStatusCode statusCode, string message, Exception innerException, bool includeFullStackTraceIfError = true)
            : base(message, innerException)
        {
            this.StatusCode = statusCode;
            this.IncludeFullStackTraceIfError = includeFullStackTraceIfError;
        }

        public HttpStatusCode StatusCode { get; }

        internal bool IncludeFullStackTraceIfError { get; }
    }
}
