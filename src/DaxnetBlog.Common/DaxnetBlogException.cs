using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Common
{
    public class DaxnetBlogException : Exception
    {
        public DaxnetBlogException() : base()
        { }

        public DaxnetBlogException(string message)
            : base(message)
        { }

        public DaxnetBlogException(string format, params object[] args)
            : base(string.Format(format, args))
        { }

        public DaxnetBlogException(string format, Exception innerException)
            : base(format, innerException)
        { }
    }
}
