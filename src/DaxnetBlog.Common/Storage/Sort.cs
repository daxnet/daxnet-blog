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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DaxnetBlog.Common.Storage
{
    /// <summary>
    /// Represents the sort specification in a query statement.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <seealso cref="System.Collections.Generic.IDictionary{System.String, DaxnetBlog.Common.Storage.SortOrder}" />
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

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count
        {
            get
            {
                return sortSpecifications.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
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
