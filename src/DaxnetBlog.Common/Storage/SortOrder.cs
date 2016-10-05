using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaxnetBlog.Common.Storage
{
    /// <summary>
    /// Represents the sort order in a sorted query.
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// Indicates that the sort is unspecified.
        /// </summary>
        Unspecified = -1,
        /// <summary>
        /// Indicates an ascending sort.
        /// </summary>
        Ascending = 0,
        /// <summary>
        /// Indicates a descending sort.
        /// </summary>
        Descending = 1
    }
}
