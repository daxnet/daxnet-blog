using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Common.Storage
{
    public interface IStorage
    {
        void Execute(Action<IDbConnection> callback);

        void Execute(Action<IDbConnection, IDbTransaction> callback, IsolationLevel iso = IsolationLevel.ReadCommitted);

        StorageDialectSettings DialectSettings { get; }
    }
}
