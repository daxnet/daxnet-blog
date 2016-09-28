using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DaxnetBlog.Common.Storage
{
    /// <summary>
    /// Represents that the implemented classes are where clause builders that
    /// build the WHERE clause for the SQL syntax for relational database systems.
    /// </summary>
    /// <typeparam name="T">The type of the data object which would be mapped to
    /// a certain table in the relational database.</typeparam>
    internal interface IWhereClauseBuilder<TEntity, TKey>
        where TKey : IEquatable<TKey>
        where TEntity : class, IEntity<TKey>, new()
    {
        /// <summary>
        /// Builds the WHERE clause from the given expression object.
        /// </summary>
        /// <param name="expression">The expression object.</param>
        /// <returns>The <c>Apworks.Storage.Builders.WhereClauseBuildResult</c> instance
        /// which contains the build result.</returns>
        WhereClauseBuildResult BuildWhereClause(Expression<Func<TEntity, bool>> expression);
    }
}
