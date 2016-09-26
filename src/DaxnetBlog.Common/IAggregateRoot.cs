using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Common
{
    public interface IAggregateRoot<TKey> : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {

    }
}
