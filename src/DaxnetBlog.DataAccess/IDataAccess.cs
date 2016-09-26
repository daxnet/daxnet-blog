using DaxnetBlog.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DaxnetBlog.DataAccess
{
    public interface IDataAccess
    {
        IEnumerable<TAggregateRoot> SelectAll<TAggregateRoot, TKey>(Expression<Func<TAggregateRoot, bool>> criteria)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : IAggregateRoot<TKey>;
    }
}
