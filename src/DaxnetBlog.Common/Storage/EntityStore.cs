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
                            .Where(p => p.PropertyType.IsSimpleType() && p.CanWrite)
                            .ToList()
                            .ForEach(x =>
                            {
                                var columnName = $"{mapping.GetTableName<TEntity, TKey>()}_{mapping.GetColumnName<TEntity, TKey>(x)}";
                                var value = reader[columnName];
                                x.SetValue(entity, EvaluatePropertyValue(x, value));
                            });
                        entities.Add(entity);
                    }
                    reader.Close();
                }
            }
            return entities;
        }

        public virtual IEnumerable<TEntity> Select<TJoined>(IDbConnection connection,
            Expression<Func<TEntity, TJoined>> includePath,
            Expression<Func<TEntity, TKey>> keySelector,
            Expression<Func<TJoined, TKey>> joinedKeySelector,
            Expression<Func<TEntity, bool>> expression = null,
            Sort<TEntity, TKey> sorting = null,
            IDbTransaction transaction = null)
            where TJoined : class, IEntity<TKey>, new()
        {
            IEnumerable<KeyValuePair<string, object>> potentialParameters;
            var sql = this.ConstructSelectStatementForJoin<TJoined>(out potentialParameters,
                keySelector,
                joinedKeySelector,
                expression, 
                sorting);

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
                    foreach (var kvp in potentialParameters)
                    {
                        var param = command.CreateParameter();
                        param.ParameterName = kvp.Key;
                        param.Value = kvp.Value;
                        command.Parameters.Add(param);
                    }
                }
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
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
                    reader.Close();
                }
            }
            return entities;
        }

        public virtual async Task<IEnumerable<TEntity>> SelectAsync<TJoined>(IDbConnection connection,
            Expression<Func<TEntity, TJoined>> includePath,
            Expression<Func<TEntity, TKey>> keySelector,
            Expression<Func<TJoined, TKey>> joinedKeySelector,
            Expression<Func<TEntity, bool>> expression = null,
            Sort<TEntity, TKey> sorting = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TJoined : class, IEntity<TKey>, new()
        {
            return await Task.FromResult(this.Select<TJoined>(connection, includePath, keySelector, joinedKeySelector, expression, sorting, transaction));
        }

        public abstract PagedResult<TEntity, TKey> Select(int pageNumber, int pageSize, 
            IDbConnection connection,
            Sort<TEntity, TKey> sorting,
            Expression<Func<TEntity, bool>> expression = null, 
            IDbTransaction transaction = null);

        public abstract Task<PagedResult<TEntity, TKey>> SelectAsync(int pageNumber, int pageSize,
            IDbConnection connection,
            Sort<TEntity, TKey> sorting,
            Expression<Func<TEntity, bool>> expression = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken));

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

        public virtual int Update(TEntity entity,
            IDbConnection connection,
            Expression<Func<TEntity, bool>> expression = null,
            IEnumerable<Expression<Func<TEntity, object>>> updateFields = null,
            IDbTransaction transaction = null)
        {
            var updateStatementConstructResult = ConstructUpdateStatement(entity, expression, updateFields);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = updateStatementConstructResult.Item1;
                if (transaction != null)
                {
                    command.Transaction = transaction;
                }
                command.Parameters.Clear();
                foreach(var c in updateStatementConstructResult.Item2)
                {
                    var param = command.CreateParameter();
                    param.ParameterName = c.Item2;
                    param.Value = c.Item3;
                    command.Parameters.Add(param);
                }
                if (updateStatementConstructResult.Item3 != null)
                {
                    foreach(var c in updateStatementConstructResult.Item3.ParameterValues)
                    {
                        var param = command.CreateParameter();
                        param.ParameterName = c.Key;
                        param.Value = c.Value;
                        command.Parameters.Add(param);
                    }
                }
                return command.ExecuteNonQuery();
            }
        }

        public virtual async Task<int> UpdateAsync(TEntity entity,
            IDbConnection connection,
            Expression<Func<TEntity, bool>> expression = null,
            IEnumerable<Expression<Func<TEntity, object>>> updateFields = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken))
            => await Task.FromResult(this.Update(entity, connection, expression, updateFields, transaction));

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
            var tableName = mapping.GetTableName<TEntity, TKey>();
            var escapedTableName = mapping.GetEscapedTableName<TEntity, TKey>(dialectSettings);
            var sqlBuilder = new StringBuilder();
            parameters = null;

            var fieldSet = string.Join(", ",
                typeof(TEntity)
                .GetTypeInfo()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.PropertyType.IsSimpleType())
                .Select(p => $"T_{tableName}.{mapping.GetEscapedColumnName<TEntity, TKey>(dialectSettings, p)} AS {tableName}_{mapping.GetColumnName<TEntity, TKey>(p)}"));
                

            sqlBuilder.AppendLine($"SELECT {fieldSet} FROM {mapping.GetEscapedTableName<TEntity, TKey>(dialectSettings)} AS T_{tableName}");

            if (expression != null)
            {
                var whereClauseBuildResult = new WhereClauseBuilder<TEntity, TKey>(mapping, dialectSettings, true).BuildWhereClause(expression);
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

        protected string ConstructSelectStatementForJoin<TJoined>(out IEnumerable<KeyValuePair<string, object>> parameters,
            Expression<Func<TEntity, TKey>> keySelector,
            Expression<Func<TJoined, TKey>> joinedKeySelector,
            Expression<Func<TEntity, bool>> expression = null,
            Sort<TEntity, TKey> sorting = null)
            where TJoined : class, IEntity<TKey>, new()
        {
            var tableName = mapping.GetTableName<TEntity, TKey>();
            var joinedTableName = mapping.GetTableName<TJoined, TKey>();
            var escapedTableName = mapping.GetEscapedTableName<TEntity, TKey>(dialectSettings);
            var joinedEscapedTableName = mapping.GetEscapedTableName<TJoined, TKey>(dialectSettings);
            var escapedKeyName = mapping.GetEscapedColumnName<TEntity, TKey>(dialectSettings, ((MemberExpression)keySelector.Body).Member.Name);
            var escapedJoinKeyName = mapping.GetEscapedColumnName<TJoined, TKey>(dialectSettings, ((MemberExpression)joinedKeySelector.Body).Member.Name);

            var sqlBuilder = new StringBuilder();
            parameters = null;

            var fieldSet = string.Join(", ",
                typeof(TEntity)
                .GetTypeInfo()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.PropertyType.IsSimpleType())
                .Select(p => $"T_{tableName}.{mapping.GetEscapedColumnName<TEntity, TKey>(dialectSettings, p)} AS {tableName}_{mapping.GetColumnName<TEntity, TKey>(p)}"));

            var joinedFieldSet = string.Join(", ",
                typeof(TJoined)
                .GetTypeInfo()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.PropertyType.IsSimpleType())
                .Select(p => $"T_{joinedTableName}.{mapping.GetEscapedColumnName<TJoined, TKey>(dialectSettings, p)} AS {joinedTableName}_{mapping.GetColumnName<TJoined, TKey>(p)}"));


            sqlBuilder.AppendLine($"SELECT {fieldSet}, {joinedFieldSet} FROM {escapedTableName} AS T_{tableName}");
            sqlBuilder.AppendLine($"JOIN  {joinedEscapedTableName} AS T_{joinedTableName}");
            sqlBuilder.AppendLine($"ON T_{tableName}.{escapedKeyName} = T_{joinedTableName}.{escapedJoinKeyName}");

            if (expression != null)
            {
                var whereClauseBuildResult = new WhereClauseBuilder<TEntity, TKey>(mapping, dialectSettings, true).BuildWhereClause(expression);
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
                .Where(p => p.CanRead && p.PropertyType.IsSimpleType());

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

        protected Tuple<string, IEnumerable<Tuple<string, string, object>>, WhereClauseBuildResult> ConstructUpdateStatement(TEntity entity,
            Expression<Func<TEntity, bool>> expression,
            IEnumerable<Expression<Func<TEntity, object>>> updateFields)
        {
            var sqlBuilder = new StringBuilder($"UPDATE {mapping.GetEscapedTableName<TEntity, TKey>(dialectSettings)} SET ");
            var columnNames = new List<Tuple<string, string, object>>();
            var updateProperties = typeof(TEntity)
                .GetTypeInfo()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.PropertyType.IsSimpleType());
            if (updateFields!=null)
            {
                updateProperties = updateProperties.Where(p =>
                    updateFields.Any(expr => ((MemberExpression)StripConvert(expr).Body).Member.Name == p.Name));
            }
            updateProperties
                .ToList()
                .ForEach(ps =>
                {
                    columnNames.Add(new Tuple<string, string, object>(mapping.GetEscapedColumnName<TEntity, TKey>(dialectSettings, ps),
                        $"{this.dialectSettings.ParameterChar}{ps.Name}", ps.GetValue(entity) ?? DBNull.Value));
                });
            for (var i = 0; i < columnNames.Count; i++)
            {
                sqlBuilder.Append($"{columnNames[i].Item1}={columnNames[i].Item2}");
                if (i < columnNames.Count - 1)
                {
                    sqlBuilder.Append(", ");
                }
            }

            WhereClauseBuildResult whereClauseBuildResult = null;
            if (expression != null)
            {
                whereClauseBuildResult = this.BuildWhereClause(expression);
                sqlBuilder.Append($" WHERE {whereClauseBuildResult.WhereClause}");
            }
            return new Tuple<string, IEnumerable<Tuple<string, string, object>>, WhereClauseBuildResult>(sqlBuilder.ToString(),
                columnNames,
                whereClauseBuildResult);
        }

        protected object EvaluatePropertyValue(PropertyInfo property, object originValue)
        {
            if (originValue == DBNull.Value)
            {
                return null;
            }

            var propertyTypeInfo = property.PropertyType.GetTypeInfo();
            if (propertyTypeInfo.IsEnum)
            {
                return Enum.ToObject(property.PropertyType, originValue);
            }

            if (propertyTypeInfo.IsGenericType &&
                propertyTypeInfo.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                propertyTypeInfo.GetGenericArguments().First().GetTypeInfo().IsEnum)
            {
                return Enum.ToObject(propertyTypeInfo.GetGenericArguments().First(), originValue);
            }

            return originValue;
        }
    }
}
