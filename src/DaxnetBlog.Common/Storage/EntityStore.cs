using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DaxnetBlog.Common.Storage
{
    public abstract class EntityStore<TEntity, TKey> : IEntityStore<TEntity, TKey>
        where TKey : IEquatable<TKey>
        where TEntity : class, IEntity<TKey>, new()
    {
        protected readonly IStoreMapping mapping;
        protected readonly StorageDialectSettings dialectSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityStore{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="mapping">The instance for providing the mappings for tableName/entityName and columnName/PropertyName. It can also
        /// provide the escaped names of mapped tabelName and columnName, based on the given <paramref name="dialectSettings"/>.</param>
        /// <param name="dialectSettings">The storage dialect settings to be used by the <paramref name="mapping"/> instance.</param>
        public EntityStore(IStoreMapping mapping, StorageDialectSettings dialectSettings)
        {
            this.mapping = mapping;
            this.dialectSettings = dialectSettings;
        }

        public virtual IEnumerable<TEntity> Select(IDbConnection connection, 
            Expression<Func<TEntity, bool>> expression = null, 
            Sort<TEntity, TKey> sorting = null, 
            IDbTransaction transaction = null)
        {
            IEnumerable<KeyValuePair<string, object>> potentialParameters;
            var sql = this.ConstructSelectStatement(out potentialParameters, expression, sorting);

            var entities = new List<TEntity>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                if (transaction != null)
                {
                    command.Transaction = transaction;
                }
                if (potentialParameters != null)
                {
                    command.Parameters.Clear();
                    foreach(var kvp in potentialParameters)
                    {
                        var param = command.CreateParameter();
                        param.ParameterName = kvp.Key;
                        param.Value = kvp.Value;
                        command.Parameters.Add(param);
                    }
                }
                using (var reader = command.ExecuteReader())
                {
                    while(reader.Read())
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
                    reader.Close();
                }
            }
            return entities;
        }

        public abstract PagedResult<TEntity, TKey> Select(int pageNumber, int pageSize, 
            IDbConnection connection,
            Sort<TEntity, TKey> sorting,
            Expression<Func<TEntity, bool>> expression = null, 
            IDbTransaction transaction = null);

        public virtual async Task<IEnumerable<TEntity>> SelectAsync(IDbConnection connection, 
            Expression<Func<TEntity, bool>> expression = null, 
            Sort<TEntity, TKey> sorting = null, 
            IDbTransaction transaction = null, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Task.FromResult(this.Select(connection, expression, sorting, transaction));
        }

        public virtual int Insert(TEntity entity, 
            IDbConnection connection, 
            IEnumerable<Expression<Func<TEntity, object>>> autoIncrementFields = null, 
            IDbTransaction transaction = null)
        {
            var sqlBuildResult = ConstructInsertStatement(entity, autoIncrementFields);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = sqlBuildResult.Item1;
                if (transaction != null)
                {
                    command.Transaction = transaction;
                }
                command.Parameters.Clear();
                foreach (var c in sqlBuildResult.Item2)
                {
                    var param = command.CreateParameter();
                    param.ParameterName = c.Item2;
                    param.Value = c.Item3;
                    command.Parameters.Add(param);
                }
                return command.ExecuteNonQuery();
            }
        }

        public virtual async Task<int> InsertAsync(TEntity entity, 
            IDbConnection connection, 
            IEnumerable<Expression<Func<TEntity, object>>> autoIncrementFields = null, 
            IDbTransaction transaction = null, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Task.FromResult(this.Insert(entity, connection, autoIncrementFields, transaction));
        }

        protected static LambdaExpression StripConvert<T>(Expression<Func<T, object>> source)
        {
            Expression result = source.Body;
            // use a loop in case there are nested Convert expressions for some crazy reason
            while (((result.NodeType == ExpressionType.Convert)
                       || (result.NodeType == ExpressionType.ConvertChecked))
                   && (result.Type == typeof(object)))
            {
                result = ((UnaryExpression)result).Operand;
            }
            return Expression.Lambda(result, source.Parameters);
        }

        protected string ConstructSelectStatement(out IEnumerable<KeyValuePair<string, object>> parameters,
            Expression<Func<TEntity, bool>> expression = null,
            Sort<TEntity, TKey> sorting = null)
        {
            var sqlBuilder = new StringBuilder();
            parameters = null;

            sqlBuilder.AppendLine($"SELECT * FROM {mapping.GetEscapedTableName<TEntity, TKey>(dialectSettings)} ");

            if (expression != null)
            {
                var whereClauseBuildResult = this.BuildWhereClause(expression);
                sqlBuilder.AppendLine($"WHERE {whereClauseBuildResult.WhereClause} ");
                parameters = new Dictionary<string, object>(whereClauseBuildResult.ParameterValues);
            }

            if (sorting != null && sorting.Count > 0)
            {
                sqlBuilder.Append("ORDER BY ");
                sqlBuilder.Append(BuildOrderByClause(sorting));
            }

            return sqlBuilder.ToString();
        }

        protected WhereClauseBuildResult BuildWhereClause(Expression<Func<TEntity, bool>> expression) =>
            new WhereClauseBuilder<TEntity, TKey>(this.mapping, this.dialectSettings).BuildWhereClause(expression);

        protected string BuildOrderByClause(Sort<TEntity, TKey> sorting)
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < sorting.Count; i++)
            {
                var sort = sorting.ElementAt(i);
                stringBuilder.AppendFormat("{0} {1}", this.mapping.GetEscapedColumnName<TEntity, TKey>(this.dialectSettings, sort.Key),
                    sort.Value == SortOrder.Descending ? "DESC" : "ASC");
                if (i < sorting.Count - 1)
                {
                    stringBuilder.Append(", ");
                }
            }
            return stringBuilder.ToString();
        }

        protected Tuple<string, IEnumerable<Tuple<string, string, object>>> ConstructInsertStatement(TEntity entity,
            IEnumerable<Expression<Func<TEntity, object>>> autoIncrementFields)
        {
            var sqlBuilder = new StringBuilder($"INSERT INTO {mapping.GetEscapedTableName<TEntity, TKey>(dialectSettings)}");
            var columnNames = new List<Tuple<string, string, object>>();
            var propertySelection = typeof(TEntity)
                .GetTypeInfo()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead);

            if (autoIncrementFields != null)
            {
                propertySelection = propertySelection.Where(p => !autoIncrementFields.Any(expr =>
                    ((MemberExpression)StripConvert(expr).Body).Member.Name == p.Name));
            }
            propertySelection
                .ToList()
                .ForEach(ps =>
                {
                    columnNames.Add(new Tuple<string, string, object>(mapping.GetEscapedColumnName<TEntity, TKey>(dialectSettings, ps),
                        $"{this.dialectSettings.ParameterChar}{ps.Name}", ps.GetValue(entity) ?? DBNull.Value));
                });
            if (autoIncrementFields != null)
            {
                sqlBuilder.Append(" (");
                sqlBuilder.Append(string.Join(", ", columnNames.Select(x => x.Item1).ToArray()));
                sqlBuilder.Append(")");
            }
            sqlBuilder.Append(" VALUES (");
            sqlBuilder.Append(string.Join(", ", columnNames.Select(x => x.Item2).ToArray()));
            sqlBuilder.Append(")");

            return new Tuple<string, IEnumerable<Tuple<string, string, object>>>(sqlBuilder.ToString(), columnNames);
        }

        
    }
}
