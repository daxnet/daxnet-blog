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
using System.Text;

namespace DaxnetBlog.Common.IntegrationServices
{
    /// <summary>
    /// Represents the key object for caching.
    /// </summary>
    /// <seealso cref="System.IEquatable{DaxnetBlog.Common.IntegrationServices.CachingKey}" />
    public sealed class CachingKey : IEquatable<CachingKey>
    {
        private const string PartSeparator = "+";
        /// <summary>
        /// Initializes a new instance of the <see cref="CachingKey"/> class.
        /// </summary>
        /// <param name="key">The key value which can represent the current object.</param>
        public CachingKey(string key)
        {
            this.Key = key;
        }

        public CachingKey(string prefix, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                var stringBuilder = new StringBuilder(prefix);
                foreach(var arg in args)
                {
                    stringBuilder.Append($"{PartSeparator}{arg}");
                }
                this.Key = stringBuilder.ToString();
            }
            else
            {
                this.Key = prefix;
            }
        }

        public string Key { get; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(CachingKey other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Key.Equals(other.Key);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as CachingKey;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Key;
        }

        public string Prefix
        {
            get
            {
                if (string.IsNullOrEmpty(Key))
                {
                    return string.Empty;
                }

                var idx = this.Key.IndexOf(PartSeparator);
                if (idx < 0)
                {
                    return this.Key;
                }

                return this.Key.Substring(0, idx);
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => string.IsNullOrEmpty(Key) ? base.GetHashCode() : Key.GetHashCode();

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator == (CachingKey a, CachingKey b)
        {
            if ((object)a == null && (object)b == null)
            {
                return true;
            }

            if ((object)a == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator != (CachingKey a, CachingKey b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="CachingKey"/>.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator CachingKey (string src)
        {
            return new CachingKey(src);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="CachingKey"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="cachingKey">The caching key.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator string (CachingKey cachingKey)
        {
            return cachingKey.Key;
        }
    }
}
