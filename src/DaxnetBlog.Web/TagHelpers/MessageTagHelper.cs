using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Web.TagHelpers
{
    [HtmlTargetElement("message", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class MessageTagHelper : TagHelper
    {
        private readonly IHtmlHelper htmlHelper;

        public MessageTagHelper(IHtmlHelper htmlHelper)
        {
            this.htmlHelper = htmlHelper;
            this.Title = "消息";
            this.Enabled = false;
            this.Type = "Info";
        }

        [HtmlAttributeName("message-title")]
        public string Title { get; set; }

        [HtmlAttributeName("message-enabled")]
        public bool Enabled { get; set; }

        [HtmlAttributeName("message-type")]
        public string Type { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (Enabled)
            {
                var tagBuilder = new TagBuilder("div");
                tagBuilder.AddCssClass("alert");
                switch(Type.ToUpper())
                {
                    case "SUCCESS":
                        tagBuilder.AddCssClass("alert-success");
                        break;
                    case "INFO":
                        tagBuilder.AddCssClass("alert-info");
                        break;
                    case "WARNING":
                        tagBuilder.AddCssClass("alert-warning");
                        break;
                    case "ERROR":
                        tagBuilder.AddCssClass("alert-danger");
                        break;
                }
                var aBuilder = new TagBuilder("a");
                aBuilder.AddCssClass("close");
                aBuilder.Attributes.Add("href", "#");
                aBuilder.Attributes.Add("data-dismiss", "alert");
                aBuilder.Attributes.Add("aria-label", "close");
                aBuilder.InnerHtml.AppendHtml("&times;");
                tagBuilder.InnerHtml.AppendHtml(aBuilder.ToHtmlString());

                var titleBuilder = new TagBuilder("strong");
                titleBuilder.InnerHtml.Append(Title);
                tagBuilder.InnerHtml.AppendHtml(titleBuilder.ToHtmlString());

                var bodyContent = (await output.GetChildContentAsync()).GetContent();
                tagBuilder.InnerHtml.AppendHtml(this.htmlHelper.Raw(bodyContent).ToString());

                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.AppendHtml(tagBuilder.ToHtmlString());
            }
        }
    }
}
