using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DaxnetBlog.Common.Storage;
using System.Data.SqlClient;
using System.Threading;

namespace DaxnetBlog.Storage.SqlServer
{
    public sealed class SqlServerStorage : Common.Storage.Storage
    {
        private static readonly StorageDialectSettings settings = new SqlServerDialectSettings();

        public SqlServerStorage(string connectionString) : base(connectionString)
        {
        }

        public override async Task ExecuteAsync(Func<IDbConnection, IDbTransaction, CancellationToken, Task> callback, IsolationLevel iso = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connection = (SqlConnection)this.CreateConnection())
            {
                await connection.OpenAsync(cancellationToken);
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

        public override async Task ExecuteAsync(Func<IDbConnection, CancellationToken, Task> callback, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connection = (SqlConnection)this.CreateConnection())
            {
                await connection.OpenAsync(cancellationToken);
                await callback(connection, cancellationToken);
            }
        }

        public override async Task<TResult> ExecuteAsync<TResult>(Func<IDbConnection, CancellationToken, Task<TResult>> callback, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connection = (SqlConnection)this.CreateConnection())
            {
                await connection.OpenAsync(cancellationToken);
                return await callback(connection, cancellationToken);
            }
        }

        public override async Task<TResult> ExecuteAsync<TResult>(Func<IDbConnection, IDbTransaction, CancellationToken, Task<TResult>> callback, IsolationLevel iso = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connection = (SqlConnection)this.CreateConnection())
            {
                await connection.OpenAsync(cancellationToken);
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

        public override StorageDialectSettings DialectSettings => settings;

        protected override IDbConnection CreateConnection() => new SqlConnection(ConnectionString);
    }
}
