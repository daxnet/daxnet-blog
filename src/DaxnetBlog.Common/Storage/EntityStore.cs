using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace DaxnetBlog.Common.Storage
{
    public abstract class EntityStore<TEntity, TKey> : IEntityStore<TEntity, TKey>
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
        protected EntityStore(IStoreMapping mapping, StorageDialectSettings dialectSettings)
        {
            this.mapping = mapping;
            this.dialectSettings = dialectSettings;
        }

        public IEnumerable<TEntity> Select(IDbConnection connection, 
            Expression<Func<TEntity, bool>> expression = null, 
            Sort<TKey, TEntity> sorting = null, 
            IDbTransaction transaction = null)
        {
            var sql = $"SELECT * FROM {mapping.GetEscapedTableName<TEntity, TKey>(dialectSettings)}";
            var entities = new List<TEntity>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                if (transaction != null)
                {
                    command.Transaction = transaction;
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
    }
}
