using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Common
{
    public interface IEntity<TKey>
        where TKey :IEquatable<TKey>
    {
        TKey Id { get; set; }
    }
}
