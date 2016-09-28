using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DaxnetBlog.Common.Storage
{
    public class EntityStore<TEntity, TKey> : IEntityStore<TEntity, TKey>
        where TKey : IEquatable<TKey>
        where TEntity : class, IEntity<TKey>, new()
    {
        private readonly IStoreMapping mapping;
        private readonly StorageDialectSettings dialectSettings;

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

        public IEnumerable<TEntity> Select(IDbConnection connection, 
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


        private string ConstructSelectStatement(out IEnumerable<KeyValuePair<string, object>> parameters,
            Expression<Func<TEntity, bool>> expression = null,
            Sort<TEntity, TKey> sorting = null)
        {
            var sqlBuilder = new StringBuilder();
            parameters = null;

            sqlBuilder.AppendLine($"SELECT * FROM {mapping.GetEscapedTableName<TEntity, TKey>(dialectSettings)} ");

            if (expression != null)
            {
                var whereClauseBuilder = new WhereClauseBuilder<TEntity, TKey>(this.mapping, this.dialectSettings);
                var whereClauseBuildResult = whereClauseBuilder.BuildWhereClause(expression);
                sqlBuilder.AppendLine($"WHERE {whereClauseBuildResult.WhereClause} ");
                parameters = new Dictionary<string, object>(whereClauseBuildResult.ParameterValues);
            }

            if (sorting != null && sorting.Count > 0)
            {
                sqlBuilder.Append("ORDER BY ");
                for (var i = 0; i < sorting.Count; i++)
                {
                    var sort = sorting.ElementAt(i);
                    sqlBuilder.AppendFormat("{0} {1}", this.mapping.GetEscapedColumnName<TEntity, TKey>(this.dialectSettings, sort.Key),
                        sort.Value == SortOrder.Descending ? "DESC" : "ASC");
                    if (i < sorting.Count - 1)
                    {
                        sqlBuilder.Append(", ");
                    }
                }
            }

            return sqlBuilder.ToString();
        }
    }
}
