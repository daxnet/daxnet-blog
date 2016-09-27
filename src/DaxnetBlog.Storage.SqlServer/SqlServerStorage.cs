using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DaxnetBlog.Common.Storage;
using System.Data.SqlClient;

namespace DaxnetBlog.Storage.SqlServer
{
    public sealed class SqlServerStorage : Common.Storage.Storage
    {
        private static readonly DialectSettings settings = new SqlServerDialectSettings();

        public SqlServerStorage(string connectionString) : base(connectionString)
        {
        }

        public override DialectSettings Settings => settings;

        protected override IDbConnection CreateConnection() => new SqlConnection(ConnectionString);
    }
}
