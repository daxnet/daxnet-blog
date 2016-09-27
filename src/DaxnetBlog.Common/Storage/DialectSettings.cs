using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Common.Storage
{
    public abstract class DialectSettings
    {
        public abstract string Name { get; }

        public abstract char ParameterChar { get; }

        public virtual string LeadingEscape => string.Empty;

        public virtual string TailingEscape => string.Empty;
    }
}
