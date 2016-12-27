using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DaxnetBlog.Web.TagHelpers
{
    [HtmlTargetElement("blog-archive-list", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class BlogArchiveListTagHelper : TagHelper
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogArchiveListTagHelper"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="urlHelperFactory">The URL helper factory.</param>
        public BlogArchiveListTagHelper(HttpClient httpClient,
            IUrlHelperFactory urlHelperFactory)
        {
            this.httpClient = httpClient;
            this.UrlHelperFactory = urlHelperFactory;
            //this.ShowIcon = false;
            //this.Icon = string.Empty;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private IUrlHelperFactory UrlHelperFactory { get; }

        [HtmlAttributeName("bal-title")]
        public string Title { get; set; }

        [HtmlAttributeName("bal-style")]
        public string Style { get; set; }

        [HtmlAttributeName("bal-show-icon")]
        public bool ShowIcon { get; set; }

        [HtmlAttributeName("bal-icon")]
        public string Icon { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            dynamic items = null;
            var urlHelper = this.UrlHelperFactory.GetUrlHelper(this.ViewContext);

            try
            {
                var result = await this.httpClient.GetAsync("blogPosts/archive/list");
                result.EnsureSuccessStatusCode();
                items = (dynamic)JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync());
            }
            catch
            {

            }

            var tagBuilder = new TagBuilder("div");
            tagBuilder.AddCssClass("panel");
            tagBuilder.AddCssClass($"panel-{Style}");

            var headerTagBuilder = new TagBuilder("div");
            headerTagBuilder.AddCssClass("panel-heading");
            if (ShowIcon)
            {
                var iconSpan = new TagBuilder("span");
                iconSpan.AddCssClass("glyphicon");
                iconSpan.AddCssClass($"glyphicon-{Icon}");
                headerTagBuilder.InnerHtml.AppendHtml(iconSpan.ToHtmlString());
            }

            headerTagBuilder.InnerHtml.AppendHtml("&nbsp;");
            headerTagBuilder.InnerHtml.Append(this.Title);

            var bodyContent = (await output.GetChildContentAsync()).GetContent();
            var bodyTagBuilder = new TagBuilder("div");
            bodyTagBuilder.Attributes.Add("id", "blogPostsArchiveList");
            bodyTagBuilder.AddCssClass("panel-body");

            //bodyTagBuilder.InnerHtml.AppendHtml(this.htmlHelper.Raw(bodyContent).ToString());
            if (items != null && items.Count > 0)
            {
                var ulTag = new TagBuilder("ul");
                foreach(dynamic item in items)
                {
                    var liTag = new TagBuilder("li");
                    var aTag = new TagBuilder("a");
                    var routeValues = new RouteValueDictionary();
                    routeValues.Add("year", item.year);
                    routeValues.Add("month", item.month);
                    aTag.Attributes.Add("href", urlHelper.Action(new UrlActionContext { Action = "archive", Controller = "blogPosts", Values = routeValues }));
                    aTag.InnerHtml.Append((string)item.text);
                    var spanTag = new TagBuilder("span");
                    spanTag.AddCssClass("badge");
                    spanTag.InnerHtml.Append(((int)item.count).ToString());
                    
                    liTag.InnerHtml.AppendHtml(aTag.ToHtmlString());
                    liTag.InnerHtml.Append(" ");
                    liTag.InnerHtml.AppendHtml(spanTag.ToHtmlString());
                    ulTag.InnerHtml.AppendHtml(liTag.ToHtmlString());
                }
                bodyTagBuilder.InnerHtml.AppendHtml(ulTag.ToHtmlString());
            }
            else
            {
                var pTag = new TagBuilder("p");
                pTag.InnerHtml.Append("无法获取历史归档信息");
                bodyTagBuilder.InnerHtml.AppendHtml(pTag.ToHtmlString());
            }

            tagBuilder.InnerHtml.AppendHtml(headerTagBuilder.ToHtmlString());
            tagBuilder.InnerHtml.AppendHtml(bodyTagBuilder.ToHtmlString());

            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(tagBuilder.ToHtmlString());
        }
    }
}
