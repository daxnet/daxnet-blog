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

        protected EntityStore(IStoreMapping mapping)
        {
            this.mapping = mapping;
        }

        public IEnumerable<TEntity> Select(DialectSettings storageProperty, 
            IDbConnection connection, 
            Expression<Func<TEntity, bool>> expression = null, 
            Sort<TKey, TEntity> sorting = null, 
            IDbTransaction transaction = null)
        {
            var sql = $"SELECT * FROM {GetEscapedTableName(storageProperty)}";
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
                                x.SetValue(entity, reader[mapping.GetColumnName<TEntity, TKey>(x)]);
                            });
                        entities.Add(entity);
                    }
                    reader.Close();
                }
            }
            return entities;
        }

        protected string GetEscapedTableName(DialectSettings dialectSettings) => $"{dialectSettings.LeadingEscape}{mapping.GetTableName<TEntity, TKey>()}{dialectSettings.TailingEscape}";

        protected string GetEscapedColumnName<TProperty>(DialectSettings dialectSettings, Expression<Func<TEntity, TProperty>> property) => $"{dialectSettings.LeadingEscape}{mapping.GetColumnName<TEntity, TKey, TProperty>(property)}{dialectSettings.TailingEscape}";

        protected string GetEscapedColumnName(DialectSettings dialectSettings, PropertyInfo propertyInfo) => $"{dialectSettings.LeadingEscape}{mapping.GetColumnName<TEntity, TKey>(propertyInfo)}{dialectSettings.TailingEscape}";
    }
}
