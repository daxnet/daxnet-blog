using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DaxnetBlog.Common.Storage
{
    public class DefaultStoreMapping : IStoreMapping
    {
        public virtual string GetTableName<TEntity, TKey>()
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new() =>
            typeof(TEntity).GetTypeInfo().Name;

        public virtual string GetColumnName<TEntity, TKey, TProperty>(Expression<Func<TEntity, TProperty>> property)
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new() =>
            ((MemberExpression)property.Body).Member.Name;

        public virtual string GetColumnName<TEntity, TKey>(PropertyInfo propertyInfo)
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new() =>
            propertyInfo.Name;

        public string GetEscapedTableName<TEntity, TKey>(StorageDialectSettings dialectSettings)
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new() =>
            $"{dialectSettings.SqlLeadingEscape}{GetTableName<TEntity, TKey>()}{dialectSettings.SqlTailingEscape}";

        public string GetEscapedColumnName<TEntity, TKey, TProperty>(StorageDialectSettings dialectSettings, Expression<Func<TEntity, TProperty>> property)
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new() =>
            $"{dialectSettings.SqlLeadingEscape}{GetColumnName<TEntity, TKey, TProperty>(property)}{dialectSettings.SqlTailingEscape}";

        public string GetEscapedColumnName<TEntity, TKey>(StorageDialectSettings dialectSettings, PropertyInfo propertyInfo)
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new() =>
            $"{dialectSettings.SqlLeadingEscape}{GetColumnName<TEntity, TKey>(propertyInfo)}{dialectSettings.SqlTailingEscape}";

        public string GetEscapedColumnName<TEntity, TKey>(StorageDialectSettings dialectSettings, string name)
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new() =>
            $"{dialectSettings.SqlLeadingEscape}{name}{dialectSettings.SqlTailingEscape}";
    }
}
