using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.WebServices.Caching
{
    public static class CachingKeys
    {
        public const string BLOGPOSTS_POST_KEY = "BLOGPOSTS_POST_KEY";
        public const string BLOGPOSTS_GETBYPAGING_KEY = "BLOGPOSTS_GETBYPAGING_KEY";
        public const string BLOGPOSTS_GETARCHIVELIST_KEY = "BLOGPOSTS_GETARCHIVELIST_KEY";
        public const string VERSION_NUMBER_KEY = "VERSION_NUMBER_KEY";
    }
}
