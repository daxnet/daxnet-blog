using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DaxnetBlog.Common.Storage
{
    public class DefaultStoreMapping : IStoreMapping
    {
        public virtual string GetTableName<TEntity, TKey>()
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new()
        {
            return typeof(TEntity).GetTypeInfo().Name;
        }

        public virtual string GetColumnName<TEntity, TKey, TProperty>(Expression<Func<TEntity, TProperty>> property)
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new()
        {
            return ((MemberExpression)property.Body).Member.Name;
        }

        public virtual string GetColumnName<TEntity, TKey>(PropertyInfo propertyInfo)
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new()
        {
            return propertyInfo.Name;
        }

        public string GetEscapedTableName<TEntity, TKey>(StorageDialectSettings dialectSettings)
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new()
        {
            return $"{dialectSettings.LeadingEscape}{GetTableName<TEntity, TKey>()}{dialectSettings.TailingEscape}";
        }

        public string GetEscapedColumnName<TEntity, TKey, TProperty>(StorageDialectSettings dialectSettings, Expression<Func<TEntity, TProperty>> property)
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new()
        {
            return $"{dialectSettings.LeadingEscape}{GetColumnName<TEntity, TKey, TProperty>(property)}{dialectSettings.TailingEscape}";
        }

        public string GetEscapedColumnName<TEntity, TKey>(StorageDialectSettings dialectSettings, PropertyInfo propertyInfo)
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new()
        {
            return $"{dialectSettings.LeadingEscape}{GetColumnName<TEntity, TKey>(propertyInfo)}{dialectSettings.TailingEscape}";
        }
    }
}
