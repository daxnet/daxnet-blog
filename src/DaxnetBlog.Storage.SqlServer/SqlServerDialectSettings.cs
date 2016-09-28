using DaxnetBlog.Common.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Storage.SqlServer
{
    internal sealed class SqlServerDialectSettings : StorageDialectSettings
    {
        public override string Name => "Microsoft SQL Server";

        public override char ParameterChar => '@';

        public override string SqlLeadingEscape => "[";

        public override string SqlTailingEscape => "]";
    }
}
