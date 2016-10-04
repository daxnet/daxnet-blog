using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Common.Storage
{
    public sealed class PagedResult<TEntity, TKey> : ICollection<TEntity>
        where TKey : IEquatable<TKey>
        where TEntity : class, IEntity<TKey>, new()
    {
        private readonly List<TEntity> entities = new List<TEntity>();

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }

        public int Count => entities.Count;

        public bool IsReadOnly => false;

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public void Add(TEntity item) => entities.Add(item);

        public void Clear() => entities.Clear();

        public bool Contains(TEntity item) => entities.Contains(item);

        public void CopyTo(TEntity[] array, int arrayIndex) => entities.CopyTo(array, arrayIndex);

        public bool Remove(TEntity item) => entities.Remove(item);

        public IEnumerator<TEntity> GetEnumerator() => entities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => entities.GetEnumerator();
    }
}
