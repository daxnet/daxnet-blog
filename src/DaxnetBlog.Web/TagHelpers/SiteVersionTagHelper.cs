using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DaxnetBlog.Web.TagHelpers
{
    [HtmlTargetElement("site-version", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class SiteVersionTagHelper : TagHelper
    {
        public SiteVersionTagHelper(IHostingEnvironment hostingEnv, HttpClient proxy)
        {
            if (string.IsNullOrEmpty(VersionString.WebSiteVersionString))
            {
                VersionString.WebSiteVersionString = File.ReadAllText(Path.Combine(hostingEnv.WebRootPath, "version.txt"));
            }

            if (string.IsNullOrEmpty(VersionString.WebServiceVersionString))
            {
                var result = proxy.GetAsync("version").Result;
                result.EnsureSuccessStatusCode();
                VersionString.WebServiceVersionString = result.Content.ReadAsStringAsync().Result;
            }
        }

        [HtmlAttributeName("sv-site-prefix")]
        public string WebSiteVersionStringPrefix { get; set; }

        [HtmlAttributeName("sv-svc-prefix")]
        public string WebServiceVersionStringPrefix { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var divBuilder = new TagBuilder("div");
            var sitePrefix = string.IsNullOrEmpty(WebSiteVersionStringPrefix) ? "Web站点版本：" : WebSiteVersionStringPrefix;
            var svcPrefix = string.IsNullOrEmpty(WebServiceVersionStringPrefix) ? "服务器版本：" : WebServiceVersionStringPrefix;
            var text = $"{sitePrefix}{VersionString.WebSiteVersionString}&nbsp;&nbsp;{svcPrefix}{VersionString.WebServiceVersionString}";
            divBuilder.InnerHtml.AppendHtml(text);
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(divBuilder.ToHtmlString());
        }
    }
}
