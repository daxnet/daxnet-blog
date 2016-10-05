using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
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

        PagedResult<TEntity, TKey> Select(int pageNumber, int pageSize,
            IDbConnection connection,
            Sort<TEntity, TKey> sorting,
            Expression<Func<TEntity, bool>> expression = null,
            IDbTransaction transaction = null);

        Task<IEnumerable<TEntity>> SelectAsync(IDbConnection connection,
            Expression<Func<TEntity, bool>> expression = null,
            Sort<TEntity, TKey> sorting = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<PagedResult<TEntity, TKey>> SelectAsync(int pageNumber, int pageSize,
            IDbConnection connection,
            Sort<TEntity, TKey> sorting,
            Expression<Func<TEntity, bool>> expression = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken));

        int Insert(TEntity entity, 
            IDbConnection connection, 
            IEnumerable<Expression<Func<TEntity, object>>> autoIncrementFields = null, 
            IDbTransaction transaction = null);

        Task<int> InsertAsync(TEntity entity,
            IDbConnection connection,
            IEnumerable<Expression<Func<TEntity, object>>> autoIncrementFields = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
