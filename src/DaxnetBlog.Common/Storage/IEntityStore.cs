// ===========================================================================================================
//      _                                 _              _       _                 
//     | |                               | |            | |     | |                
//   __| |   __ _  __  __  _ __     ___  | |_   ______  | |__   | |   ___     __ _ 
//  / _` |  / _` | \ \/ / | '_ \   / _ \ | __| |______| | '_ \  | |  / _ \   / _` |
// | (_| | | (_| |  >  <  | | | | |  __/ | |_           | |_) | | | | (_) | | (_| |
//  \__,_|  \__,_| /_/\_\ |_| |_|  \___|  \__|          |_.__/  |_|  \___/   \__, |
//                                                                            __/ |
//                                                                           |___/ 
//
// 
// Daxnet Personal Blog
// Copyright © 2016 by daxnet (Sunny Chen)
//
// https://github.com/daxnet/daxnet-blog
//
// MIT License
// 
// Copyright(c) 2016 Sunny Chen
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ===========================================================================================================

using System;
using System.Collections.Generic;
using System.Data;
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

        IEnumerable<TEntity> Select<TJoined>(IDbConnection connection,
            Expression<Func<TEntity, TJoined>> includePath,
            Expression<Func<TEntity, TKey>> keySelector,
            Expression<Func<TJoined, TKey>> joinedKeySelector,
            Expression<Func<TEntity, bool>> expression = null,
            Sort<TEntity, TKey> sorting = null,
            IDbTransaction transaction = null)
            where TJoined : class, IEntity<TKey>, new();

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

        Task<IEnumerable<TEntity>> SelectAsync<TJoined>(IDbConnection connection,
            Expression<Func<TEntity, TJoined>> includePath,
            Expression<Func<TEntity, TKey>> keySelector,
            Expression<Func<TJoined, TKey>> joinedKeySelector,
            Expression<Func<TEntity, bool>> expression = null,
            Sort<TEntity, TKey> sorting = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken))
            where TJoined : class, IEntity<TKey>, new();

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

        int Update(TEntity entity,
            IDbConnection connection,
            Expression<Func<TEntity, bool>> expression = null,
            IEnumerable<Expression<Func<TEntity, object>>> updateFields = null,
            IDbTransaction transaction = null);

        Task<int> UpdateAsync(TEntity entity,
            IDbConnection connection,
            Expression<Func<TEntity, bool>> expression = null,
            IEnumerable<Expression<Func<TEntity, object>>> updateFields = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
