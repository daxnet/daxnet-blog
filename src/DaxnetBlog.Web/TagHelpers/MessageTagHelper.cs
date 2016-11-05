using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
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
            //if (Enabled)
            //{
                var tagBuilder = new TagBuilder("div");
                tagBuilder.AddCssClass("alert");
                if (!Enabled)
                {
                    tagBuilder.Attributes.Add("style", "display: none;");
                }
                tagBuilder.Attributes.Add("id", "message-alert");
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
                aBuilder.Attributes.Add("data-hide", "alert");
                aBuilder.Attributes.Add("aria-label", "close");
                aBuilder.InnerHtml.AppendHtml("&times;");
                tagBuilder.InnerHtml.AppendHtml(aBuilder.ToHtmlString());

                var titleBuilder = new TagBuilder("strong");
                titleBuilder.Attributes.Add("id", "message-alert-title");
                titleBuilder.InnerHtml.Append($"{Title}：");
                tagBuilder.InnerHtml.AppendHtml(titleBuilder.ToHtmlString());

                var bodyContent = (await output.GetChildContentAsync()).GetContent();
                var spanBuilder = new TagBuilder("span");
                spanBuilder.Attributes.Add("id", "message-alert-body");
                spanBuilder.InnerHtml.AppendHtml(bodyContent);

                tagBuilder.InnerHtml.AppendHtml(spanBuilder.ToHtmlString());

                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.AppendHtml(tagBuilder.ToHtmlString());
            //}
        }
    }
}
