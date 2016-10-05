using DaxnetBlog.Common.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;

namespace DaxnetBlog.Domain.EntityStore
{
    public sealed class PluralTableNameStoreMapping : DefaultStoreMapping
    {
        public override string GetTableName<TEntity, TKey>()
        {
            return base.GetTableName<TEntity, TKey>().Pluralize();
        }
    }
}
