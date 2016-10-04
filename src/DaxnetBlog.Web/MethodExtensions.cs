using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DaxnetBlog.Web
{
    public static class MethodExtensions
    {
        public static string ToHtmlString(this TagBuilder tb)
        {
            using (var writer = new StringWriter())
            {
                tb.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();
            }
        }
    }
}
