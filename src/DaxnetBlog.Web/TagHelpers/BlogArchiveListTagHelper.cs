using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
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

        public BlogArchiveListTagHelper(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        [HtmlAttributeName("sc-title")]
        public string Title { get; set; }

        [HtmlAttributeName("sc-style")]
        public string Style { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            dynamic items = null;
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
                    aTag.Attributes.Add("href", $"blogPosts/{item.year}/{item.month}");
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
