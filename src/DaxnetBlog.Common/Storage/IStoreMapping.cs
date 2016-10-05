using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace DaxnetBlog.Common.Storage
{
    public interface IStoreMapping
    {
        string GetTableName<TEntity, TKey>()
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new();

        string GetColumnName<TEntity, TKey, TProperty>(Expression<Func<TEntity, TProperty>> property)
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new();

        string GetColumnName<TEntity, TKey>(PropertyInfo propertyInfo)
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new();

        string GetEscapedTableName<TEntity, TKey>(StorageDialectSettings dialectSettings)
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new();

        string GetEscapedColumnName<TEntity, TKey, TProperty>(StorageDialectSettings dialectSettings, Expression<Func<TEntity, TProperty>> property)
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new();

        string GetEscapedColumnName<TEntity, TKey>(StorageDialectSettings dialectSettings, PropertyInfo propertyInfo)
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new();

        string GetEscapedColumnName<TEntity, TKey>(StorageDialectSettings dialectSettings, string name)
            where TKey : IEquatable<TKey>
            where TEntity : class, IEntity<TKey>, new();
    }
}
