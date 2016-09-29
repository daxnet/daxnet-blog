using DaxnetBlog.Common;
using DaxnetBlog.Common.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Reflection;
using System.Data.SqlClient;

namespace DaxnetBlog.Storage.SqlServer
{
    public sealed class SqlServerEntityStore<TEntity, TKey> : EntityStore<TEntity, TKey>
        where TKey : IEquatable<TKey>
        where TEntity : class, IEntity<TKey>, new()
    {
        public SqlServerEntityStore(IStoreMapping mapping, StorageDialectSettings dialectSettings) : base(mapping, dialectSettings)
        {
        }

        public override async Task<IEnumerable<TEntity>> SelectAsync(IDbConnection connection, 
            Expression<Func<TEntity, bool>> expression = null, 
            Sort<TEntity, TKey> sorting = null, IDbTransaction transaction = null, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<KeyValuePair<string, object>> potentialParameters;
            var sql = this.ConstructSelectStatement(out potentialParameters, expression, sorting);

            var entities = new List<TEntity>();
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = sql;
                if (transaction != null)
                {
                    command.Transaction = (SqlTransaction)transaction;
                }
                if (potentialParameters != null)
                {
                    command.Parameters.Clear();
                    foreach (var kvp in potentialParameters)
                    {
                        var param = command.CreateParameter();
                        param.ParameterName = kvp.Key;
                        param.Value = kvp.Value;
                        command.Parameters.Add(param);
                    }
                }
                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var entity = new TEntity();
                        typeof(TEntity)
                            .GetTypeInfo()
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => p.CanWrite)
                            .ToList()
                            .ForEach(x =>
                            {
                                var value = reader[mapping.GetColumnName<TEntity, TKey>(x)];
                                if (value == DBNull.Value)
                                {
                                    value = null;
                                }
                                x.SetValue(entity, value);
                            });
                        entities.Add(entity);
                    }
                }
            }
            return entities;
        }

        public override async Task<int> InsertAsync(TEntity entity, IDbConnection connection, IEnumerable<Expression<Func<TEntity, object>>> autoIncrementFields = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var sqlBuildResult = ConstructInsertStatement(entity, autoIncrementFields);

            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = sqlBuildResult.Item1;
                if (transaction != null)
                {
                    command.Transaction = (SqlTransaction)transaction;
                }
                command.Parameters.Clear();
                foreach (var c in sqlBuildResult.Item2)
                {
                    var param = command.CreateParameter();
                    param.ParameterName = c.Item2;
                    param.Value = c.Item3;
                    command.Parameters.Add(param);
                }
                return await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }
    }
}
