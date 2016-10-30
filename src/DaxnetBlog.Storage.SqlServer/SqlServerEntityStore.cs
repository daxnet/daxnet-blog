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
using System.Text;

namespace DaxnetBlog.Storage.SqlServer
{
    /// <summary>
    /// Represents the SQL Server entity store implementation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <seealso cref="DaxnetBlog.Common.Storage.EntityStore{TEntity, TKey}" />
    public sealed class SqlServerEntityStore<TEntity, TKey> : EntityStore<TEntity, TKey>
        where TKey : IEquatable<TKey>
        where TEntity : class, IEntity<TKey>, new()
    {
        public SqlServerEntityStore(IStoreMapping mapping, StorageDialectSettings dialectSettings) : base(mapping, dialectSettings)
        {
        }

        public override PagedResult<TEntity, TKey> Select(int pageNumber, int pageSize, 
            IDbConnection connection,
            Sort<TEntity, TKey> sorting,
            Expression<Func<TEntity, bool>> expression = null, 
            IDbTransaction transaction = null)
        {
            WhereClauseBuildResult whereClauseBuildResult = expression != null ? this.BuildWhereClause(expression) : null;
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"SELECT (SELECT COUNT(*) FROM {this.mapping.GetEscapedTableName<TEntity, TKey>(this.dialectSettings)}");
            if (whereClauseBuildResult != null)
            {
                sqlBuilder.Append($" WHERE {whereClauseBuildResult.WhereClause} ");
            }
            sqlBuilder.AppendLine(") AS _TotalNumberOfRecords,");
            sqlBuilder.AppendLine($" * FROM (SELECT ROW_NUMBER() OVER (ORDER BY {this.BuildOrderByClause(sorting)}) AS _RowNumber, * FROM {this.mapping.GetEscapedTableName<TEntity, TKey>(this.dialectSettings)}");
            if (whereClauseBuildResult != null)
            {
                sqlBuilder.AppendLine($" WHERE {whereClauseBuildResult.WhereClause} ");
            }
            sqlBuilder.AppendLine($") AS PagedResult WHERE _RowNumber >= {(pageNumber - 1) * pageSize + 1} AND _RowNumber < {pageNumber * pageSize + 1} ORDER BY _RowNumber");
            var sql = sqlBuilder.ToString();

            var pagedResult = new PagedResult<TEntity, TKey>
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                if (transaction != null)
                {
                    command.Transaction = transaction;
                }
                if (whereClauseBuildResult != null)
                {
                    command.Parameters.Clear();
                    foreach (var kvp in whereClauseBuildResult.ParameterValues)
                    {
                        var param = command.CreateParameter();
                        param.ParameterName = kvp.Key;
                        param.Value = kvp.Value;
                        command.Parameters.Add(param);
                    }
                }
                using (var reader = command.ExecuteReader())
                {
                    var totalNumOfRecordsRead = false;
                    while (reader.Read())
                    {
                        if (!totalNumOfRecordsRead)
                        {
                            pagedResult.TotalRecords = Convert.ToInt32(reader["_TotalNumberOfRecords"]);
                            pagedResult.TotalPages = (pagedResult.TotalRecords - 1) / pageSize + 1;
                            totalNumOfRecordsRead = true;
                        }
                        var entity = new TEntity();
                        typeof(TEntity)
                            .GetTypeInfo()
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => p.CanWrite && p.PropertyType.IsSimpleType())
                            .ToList()
                            .ForEach(x =>
                            {
                                var value = reader[mapping.GetColumnName<TEntity, TKey>(x)];
                                x.SetValue(entity, EvaluatePropertyValue(x, value));
                            });
                        pagedResult.Add(entity);
                    }
                    reader.Close();
                }
            }

            return pagedResult;
        }

        public override async Task<PagedResult<TEntity, TKey>> SelectAsync(int pageNumber, int pageSize, IDbConnection connection, Sort<TEntity, TKey> sorting, Expression<Func<TEntity, bool>> expression = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            WhereClauseBuildResult whereClauseBuildResult = expression != null ? this.BuildWhereClause(expression) : null;
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"SELECT (SELECT COUNT(*) FROM {this.mapping.GetEscapedTableName<TEntity, TKey>(this.dialectSettings)}");
            if (whereClauseBuildResult != null)
            {
                sqlBuilder.Append($" WHERE {whereClauseBuildResult.WhereClause} ");
            }
            sqlBuilder.AppendLine(") AS _TotalNumberOfRecords,");
            sqlBuilder.AppendLine($" * FROM (SELECT ROW_NUMBER() OVER (ORDER BY {this.BuildOrderByClause(sorting)}) AS _RowNumber, * FROM {this.mapping.GetEscapedTableName<TEntity, TKey>(this.dialectSettings)}");
            if (whereClauseBuildResult != null)
            {
                sqlBuilder.AppendLine($" WHERE {whereClauseBuildResult.WhereClause} ");
            }
            sqlBuilder.AppendLine($") AS PagedResult WHERE _RowNumber >= {(pageNumber - 1) * pageSize + 1} AND _RowNumber < {pageNumber * pageSize + 1} ORDER BY _RowNumber");
            var sql = sqlBuilder.ToString();

            var pagedResult = new PagedResult<TEntity, TKey>
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = sql;
                if (transaction != null)
                {
                    command.Transaction = (SqlTransaction)transaction;
                }
                if (whereClauseBuildResult != null)
                {
                    command.Parameters.Clear();
                    foreach (var kvp in whereClauseBuildResult.ParameterValues)
                    {
                        var param = command.CreateParameter();
                        param.ParameterName = kvp.Key;
                        param.Value = kvp.Value;
                        command.Parameters.Add(param);
                    }
                }
                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    var totalNumOfRecordsRead = false;
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        if (!totalNumOfRecordsRead)
                        {
                            pagedResult.TotalRecords = Convert.ToInt32(reader["_TotalNumberOfRecords"]);
                            pagedResult.TotalPages = (pagedResult.TotalRecords - 1) / pageSize + 1;
                            totalNumOfRecordsRead = true;
                        }
                        var entity = new TEntity();
                        typeof(TEntity)
                            .GetTypeInfo()
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => p.CanWrite && p.PropertyType.IsSimpleType())
                            .ToList()
                            .ForEach(x =>
                            {
                                var value = reader[mapping.GetColumnName<TEntity, TKey>(x)];
                                x.SetValue(entity, EvaluatePropertyValue(x, value));
                            });
                        pagedResult.Add(entity);
                    }
                }
            }

            return pagedResult;
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
                            .Where(p => p.CanWrite && p.PropertyType.IsSimpleType())
                            .ToList()
                            .ForEach(x =>
                            {
                                var columnName = $"{mapping.GetTableName<TEntity, TKey>()}_{mapping.GetColumnName<TEntity, TKey>(x)}";
                                var value = reader[columnName];
                                x.SetValue(entity, EvaluatePropertyValue(x, value));
                            });
                        entities.Add(entity);
                    }
                }
            }
            return entities;
        }

        public override async Task<IEnumerable<TEntity>> SelectAsync<TJoined>(IDbConnection connection, Expression<Func<TEntity, TJoined>> includePath, Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TJoined, TKey>> joinedKeySelector, Expression<Func<TEntity, bool>> expression = null, Sort<TEntity, TKey> sorting = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<KeyValuePair<string, object>> potentialParameters;
            var sql = this.ConstructSelectStatementForJoin<TJoined>(out potentialParameters,
                keySelector,
                joinedKeySelector,
                expression,
                sorting);

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
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var entity = new TEntity();
                        var joined = new TJoined();
                        typeof(TEntity)
                            .GetTypeInfo()
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => p.CanWrite)
                            .ToList()
                            .ForEach(x =>
                            {
                                if (x.PropertyType.IsSimpleType())
                                {
                                    var columnName = $"{mapping.GetTableName<TEntity, TKey>()}_{mapping.GetColumnName<TEntity, TKey>(x)}";
                                    var value = reader[columnName];
                                    x.SetValue(entity, EvaluatePropertyValue(x, value));
                                }
                                else if (x.PropertyType == typeof(TJoined))
                                {
                                    typeof(TJoined)
                                        .GetTypeInfo()
                                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                        .Where(p => p.CanWrite && p.PropertyType.IsSimpleType())
                                        .ToList()
                                        .ForEach(y =>
                                        {
                                            var joinedColumnName = $"{mapping.GetTableName<TJoined, TKey>()}_{mapping.GetColumnName<TJoined, TKey>(y)}";
                                            var joinedValue = reader[joinedColumnName];
                                            y.SetValue(joined, EvaluatePropertyValue(y, joinedValue));
                                        });
                                    x.SetValue(entity, EvaluatePropertyValue(x, joined));
                                }
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

        public override async Task<int> UpdateAsync(TEntity entity, IDbConnection connection, Expression<Func<TEntity, bool>> expression = null, IEnumerable<Expression<Func<TEntity, object>>> updateFields = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var updateStatementConstructResult = ConstructUpdateStatement(entity, expression, updateFields);
            using (var command = (SqlCommand)connection.CreateCommand())
            {
                command.CommandText = updateStatementConstructResult.Item1;
                if (transaction != null)
                {
                    command.Transaction = (SqlTransaction)transaction;
                }
                command.Parameters.Clear();
                foreach (var c in updateStatementConstructResult.Item2)
                {
                    var param = command.CreateParameter();
                    param.ParameterName = c.Item2;
                    param.Value = c.Item3;
                    command.Parameters.Add(param);
                }
                if (updateStatementConstructResult.Item3 != null)
                {
                    foreach (var c in updateStatementConstructResult.Item3.ParameterValues)
                    {
                        var param = command.CreateParameter();
                        param.ParameterName = c.Key;
                        param.Value = c.Value;
                        command.Parameters.Add(param);
                    }
                }
                return await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }
    }
}
