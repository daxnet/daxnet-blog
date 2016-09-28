using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DaxnetBlog.Common.Storage
{
    public interface IEntityStore<TEntity, TKey>
        where TKey : IEquatable<TKey>
        where TEntity : class, IEntity<TKey>, new()
    {
        IEnumerable<TEntity> Select(IDbConnection connection, 
            Expression<Func<TEntity, bool>> expression = null,
            Sort<TEntity, TKey> sorting = null,
            IDbTransaction transaction = null);
    }
}
