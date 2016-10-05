using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Web
{
    public class WebsiteSettings
    {
        public string BaseUri { get; set; }

        public int BlogPostsPageSize { get; set; }

        public int TruncateSummaryLength { get; set; }
    }
}
