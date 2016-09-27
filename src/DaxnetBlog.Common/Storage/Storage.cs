using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public void Execute(Action<IDbConnection> callback)
        {
            using (var connection = this.CreateConnection())
            {
                connection.Open();
                callback(connection);
            }
        }

        public void Execute(Action<IDbConnection, IDbTransaction> callback, IsolationLevel iso = IsolationLevel.ReadCommitted)
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

        protected abstract IDbConnection CreateConnection();
    }
}
