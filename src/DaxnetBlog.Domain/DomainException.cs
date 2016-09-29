using DaxnetBlog.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Domain
{
    public class DomainException : DaxnetBlogException
    {
        public DomainException() : base()
        { }

        public DomainException(string message)
            : base(message)
        { }

        public DomainException(string format, params object[] args)
            : base(string.Format(format, args))
        { }

        public DomainException(string format, Exception innerException)
            : base(format, innerException)
        { }
    }
}
