using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Web.TagHelpers
{
    internal sealed class PagerSegment
    {
        public int StartPageIndex { get; set; }
        public int EndPageIndex { get; set; }

        public PagerSegment()
        {
        }

        public PagerSegment(int startPageIndex, int endPageIndex)
        {
            this.StartPageIndex = startPageIndex;
            this.EndPageIndex = endPageIndex;
        }
    }
}
