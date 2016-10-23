using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Common.Storage
{
    /// <summary>
    /// Represents the dialect settings and information for a particular type of RDBMS.
    /// </summary>
    public abstract class StorageDialectSettings
    {
        /// <summary>
        /// The name of the dialect, for example, Microsoft SQL Server.
        /// </summary>
        public abstract string Name { get; }

        public abstract char ParameterChar { get; }

        public virtual string SqlLeadingEscape => string.Empty;

        public virtual string SqlTailingEscape => string.Empty;

        public virtual string SqlAndStatement => "AND";

        public virtual string SqlOrStatement => "OR";

        public virtual string SqlEqualOperator => "=";

        public virtual string SqlIsOperator => "IS";

        public virtual string SqlIsNotOperator => "IS NOT";

        public virtual string SqlNotStatement => "NOT";

        public virtual string SqlNotEqualOperator => "<>";

        public virtual string SqlLikeStatement => "LIKE";

        public virtual char SqlLikeSymbol => '%';
    }
}
