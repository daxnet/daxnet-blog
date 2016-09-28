using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DaxnetBlog.Common.Storage
{
    public sealed class Sort<TEntity, TKey> : IDictionary<string, SortOrder>
        where TKey : IEquatable<TKey>
        where TEntity : class, IEntity<TKey>
    {
        #region Nested Internal Classes
        private sealed class DumpMemberAccessNameVisitor : ExpressionVisitor
        {
            private List<string> nameList = new List<string>();
            protected override Expression VisitMember(MemberExpression node)
            {
                var expression = base.VisitMember(node);
                nameList.Add(node.Member.Name);
                return expression;
            }

            public string MemberAccessName => string.Join(".", nameList);

            public override string ToString() => MemberAccessName;
        }
        #endregion 

        private readonly Dictionary<string, SortOrder> sortSpecifications = new Dictionary<string, SortOrder>();

        public static readonly Sort<TEntity, TKey> None = new Sort<TEntity, TKey>() { { x => x.Id, SortOrder.Unspecified } };

        public SortOrder this[string key]
        {
            get
            {
                return sortSpecifications[key];
            }

            set
            {
                sortSpecifications[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return sortSpecifications.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return sortSpecifications.Keys;
            }
        }

        public ICollection<SortOrder> Values
        {
            get
            {
                return sortSpecifications.Values;
            }
        }

        private static Expression<Func<TEntity, object>> CreateLambdaExpression(string propertyName)
        {
            var param = Expression.Parameter(typeof(TEntity), "x");
            Expression body = param;
            foreach (var member in propertyName.Split('.'))
            {
                body = Expression.Property(body, member);
            }
            return Expression.Lambda<Func<TEntity, object>>(Expression.Convert(body, typeof(object)), param);
        }

        public void Add(KeyValuePair<string, SortOrder> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(string key, SortOrder value)
        {
            sortSpecifications.Add(key, value);
        }

        public void Add(Expression<Func<TEntity, object>> sortExpression, SortOrder sortOrder)
        {
            var visitor = new DumpMemberAccessNameVisitor();
            visitor.Visit(sortExpression);
            var memberAccessName = visitor.MemberAccessName;
            if (!ContainsKey(memberAccessName))
            {
                Add(memberAccessName, sortOrder);
            }
        }

        public IEnumerable<Tuple<Expression<Func<TEntity, object>>, SortOrder>> Specifications
        {
            get
            {
                foreach(var kvp in sortSpecifications)
                {
                    yield return new Tuple<Expression<Func<TEntity, object>>, SortOrder>(CreateLambdaExpression(kvp.Key), kvp.Value);
                }
            }
        }

        public void Clear()
        {
            sortSpecifications.Clear();
        }

        public bool Contains(KeyValuePair<string, SortOrder> item)
        {
            return sortSpecifications.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return sortSpecifications.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, SortOrder>[] array, int arrayIndex)
        {
            ((ICollection)sortSpecifications).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, SortOrder>> GetEnumerator()
        {
            return sortSpecifications.GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, SortOrder> item)
        {
            return sortSpecifications.Remove(item.Key);
        }

        public bool Remove(string key)
        {
            return sortSpecifications.Remove(key);
        }

        public bool TryGetValue(string key, out SortOrder value)
        {
            return sortSpecifications.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return sortSpecifications.GetEnumerator();
        }
    }
}
