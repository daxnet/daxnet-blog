using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DaxnetBlog.Common.Storage
{
    public interface IStorage
    {
        TResult Execute<TResult>(Func<IDbConnection, TResult> callback);

        Task<TResult> ExecuteAsync<TResult>(Func<IDbConnection, CancellationToken, Task<TResult>> callback, CancellationToken cancellationToken = default(CancellationToken));

        void Execute(Action<IDbConnection> callback);

        Task ExecuteAsync(Func<IDbConnection, CancellationToken, Task> callback, CancellationToken cancellationToken = default(CancellationToken));

        void Execute(Action<IDbConnection, IDbTransaction> callback, IsolationLevel iso = IsolationLevel.ReadCommitted);

        Task ExecuteAsync(Func<IDbConnection, IDbTransaction, CancellationToken, Task> callback, IsolationLevel iso = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default(CancellationToken));

        TResult Execute<TResult>(Func<IDbConnection, IDbTransaction, TResult> callback, IsolationLevel iso = IsolationLevel.ReadCommitted);

        Task<TResult> ExecuteAsync<TResult>(Func<IDbConnection, IDbTransaction, CancellationToken, Task<TResult>> callback, IsolationLevel iso = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default(CancellationToken));

        StorageDialectSettings DialectSettings { get; }
    }
}
