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

        public abstract DialectSettings Settings { get; }

        public void Execute(Action<DialectSettings, IDbConnection> callback)
        {
            using (var connection = this.CreateConnection())
            {
                connection.Open();
                callback(this.Settings, connection);
            }
        }

        public void Execute(Action<DialectSettings, IDbConnection, IDbTransaction> callback, IsolationLevel iso = IsolationLevel.ReadCommitted)
        {
            using (var connection = this.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(iso))
                {
                    try
                    {
                        callback(this.Settings, connection, transaction);
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
