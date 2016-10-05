using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DaxnetBlog.Common.Storage
{
    public abstract class Storage : IStorage
    {
        private readonly Guid id = Guid.NewGuid();
        private readonly string connectionString;

        public Storage(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected string ConnectionString => this.connectionString;

        public Guid Id => id;

        public abstract StorageDialectSettings DialectSettings { get; }

        public virtual void Execute(Action<IDbConnection> callback)
        {
            using (var connection = this.CreateConnection())
            {
                connection.Open();
                callback(connection);
            }
        }

        public virtual TResult Execute<TResult>(Func<IDbConnection, TResult> callback)
        {
            using (var connection = this.CreateConnection())
            {
                connection.Open();
                return callback(connection);
            }
        }

        public virtual void Execute(Action<IDbConnection, IDbTransaction> callback, IsolationLevel iso = IsolationLevel.ReadCommitted)
        {
            using (var connection = this.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(iso))
                {
                    try
                    {
                        callback(connection, transaction);
                        transaction.Commit();
                    }
                    catch
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch { throw; }
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public virtual TResult Execute<TResult>(Func<IDbConnection, IDbTransaction, TResult> callback, IsolationLevel iso = IsolationLevel.ReadCommitted)
        {
            using (var connection = this.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(iso))
                {
                    try
                    {
                        var result = callback(connection, transaction);
                        transaction.Commit();
                        return result;
                    }
                    catch
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch { throw; }
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public virtual async Task<TResult> ExecuteAsync<TResult>(Func<IDbConnection, CancellationToken, Task<TResult>> callback, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connection = this.CreateConnection())
            {
                connection.Open();
                return await callback(connection, cancellationToken);
            }
        }

        public virtual async Task ExecuteAsync(Func<IDbConnection, CancellationToken, Task> callback, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connection = this.CreateConnection())
            {
                connection.Open();
                await callback(connection, cancellationToken);
            }
        }

        public virtual async Task ExecuteAsync(Func<IDbConnection, IDbTransaction, CancellationToken, Task> callback, IsolationLevel iso = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connection = this.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(iso))
                {
                    try
                    {
                        await callback(connection, transaction, cancellationToken);
                        transaction.Commit();
                    }
                    catch
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch { throw; }
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public virtual async Task<TResult> ExecuteAsync<TResult>(Func<IDbConnection, IDbTransaction, CancellationToken, Task<TResult>> callback, IsolationLevel iso = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connection = this.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(iso))
                {
                    try
                    {
                        var result = await callback(connection, transaction, cancellationToken);
                        transaction.Commit();
                        return result;
                    }
                    catch
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch { throw; }
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        protected abstract IDbConnection CreateConnection();
    }
}
