using DaxnetBlog.Common.IntegrationServices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.WebServices.Caching
{
    public class MemoryCachingService : ICachingService
    {
        private readonly ConcurrentDictionary<CachingKey, object> cache = new ConcurrentDictionary<CachingKey, object>();

        public object Get(CachingKey key)
        {
            object ret = null;
            if (cache.TryGetValue(key, out ret))
            {
                return ret;
            }
            return null;
        }

        public void Put(CachingKey key, object value)
        {
            cache.AddOrUpdate(key, value, (k, v) => v);
        }

        public void Delete(CachingKey key)
        {
            object temp = null;
            cache.TryRemove(key, out temp);
        }

        public void DeleteByPrefix(string prefix)
        {
            var keys = cache.Keys.ToList().Where(k => k.Prefix.Equals(prefix, StringComparison.CurrentCultureIgnoreCase));
            object temp = null;
            foreach(var key in keys)
            {
                cache.TryRemove(key, out temp);
            }
        }
    }
}
