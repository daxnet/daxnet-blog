using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaxnetBlog.Web;
using Microsoft.AspNetCore.Routing;
using System.Text;

namespace DaxnetBlog.Web.TagHelpers
{
    [HtmlTargetElement("pager", TagStructure = TagStructure.WithoutEndTag)]
    public class PagerTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private IUrlHelperFactory UrlHelperFactory { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagerTagHelper"/> class.
        /// </summary>
        /// <param name="urlHelperFactory">The URL helper factory.</param>
        public PagerTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            this.UrlHelperFactory = urlHelperFactory;
            this.VisibleNumbers = 5;
            this.ParameterName = "page";
        }

        /// <summary>
        /// Gets or sets the index of current page. This value should
        /// be larger than or equal to 1.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
        [HtmlAttributeName("pager-current-page")]
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the number of total pages.
        /// </summary>
        /// <value>
        /// The total pages.
        /// </value>
        [HtmlAttributeName("pager-total-pages")]
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the visible numbers.
        /// </summary>
        /// <value>
        /// The visible numbers.
        /// </value>
        [HtmlAttributeName("pager-visible-numbers")]
        public int VisibleNumbers { get; set; }

        /// <summary>
        /// Gets or sets the controller name
        /// </summary>
        /// <value>
        /// The controller.
        /// </value>
        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        /// <summary>
        /// Gets or sets the action name
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the name of the parameter in the controller action.
        /// </summary>
        /// <value>
        /// The name of the parameter.
        /// </value>
        [HtmlAttributeName("pager-parameter-name")]
        public string ParameterName { get; set; }

        /// <summary>
        /// Gets or sets the name of the anchor tag within the HTML document, that when
        /// the link is clicked, just get the updated page and locate the document to the
        /// place where the anchor has been set.
        /// </summary>
        /// <value>
        /// The name of the anchor tag.
        /// </value>
        [HtmlAttributeName("pager-anchor-tag-name")]
        public string AnchorTagName { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (this.TotalPages <= 0)
            {
                return;
            }

            var routeValues = new RouteValueDictionary();

            var divBuilder = new TagBuilder("div");
            divBuilder.AddCssClass("text-center");
            //var pageNumberStringBuilder = new StringBuilder();
            var urlHelper = this.UrlHelperFactory.GetUrlHelper(this.ViewContext);

            var ulTag = new TagBuilder("ul");
            ulTag.AddCssClass("pagination");
            ulTag.AddCssClass("pagination-sm");
            TagBuilder liTag = null;
            
            TagBuilder firstPageTagBuilder;
            TagBuilder prevPageTagBuilder;
            if (this.CurrentPage > 1)
            {
                firstPageTagBuilder = new TagBuilder("a");
                firstPageTagBuilder.InnerHtml.Append("首页");
                firstPageTagBuilder.Attributes.Add("href",
                    string.IsNullOrEmpty(AnchorTagName)
                        ? urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller })
                        : string.Format("{0}#{1}", urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller }), AnchorTagName));
                liTag = new TagBuilder("li");
                liTag.InnerHtml.AppendHtml(firstPageTagBuilder.ToHtmlString());
                ulTag.InnerHtml.AppendHtml(liTag.ToHtmlString());
                

                prevPageTagBuilder = new TagBuilder("a");
                prevPageTagBuilder.InnerHtml.Append("上一页");
                routeValues[ParameterName] = CurrentPage - 1;
                prevPageTagBuilder.Attributes.Add("href",
                    string.IsNullOrEmpty(AnchorTagName)
                        ? urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller, Values = routeValues })
                        : string.Format("{0}#{1}", urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller, Values = routeValues }), AnchorTagName));
                liTag = new TagBuilder("li");
                liTag.InnerHtml.AppendHtml(prevPageTagBuilder.ToHtmlString());
                ulTag.InnerHtml.AppendHtml(liTag.ToHtmlString());

            }
            else
            {
                firstPageTagBuilder = new TagBuilder("a");
                firstPageTagBuilder.InnerHtml.Append("首页");
                liTag = new TagBuilder("li");
                liTag.AddCssClass("disabled");
                liTag.InnerHtml.AppendHtml(firstPageTagBuilder.ToHtmlString());
                ulTag.InnerHtml.AppendHtml(liTag.ToHtmlString());


                prevPageTagBuilder = new TagBuilder("a");
                prevPageTagBuilder.InnerHtml.Append("上一页");
                liTag = new TagBuilder("li");
                liTag.AddCssClass("disabled");
                liTag.InnerHtml.AppendHtml(prevPageTagBuilder.ToHtmlString());
                ulTag.InnerHtml.AppendHtml(liTag.ToHtmlString());
            }

            var pagerSegments = new List<PagerSegment>();
            if (TotalPages <= VisibleNumbers)
            {
                pagerSegments.Add(new PagerSegment { StartPageIndex = 1, EndPageIndex = TotalPages });
            }
            else
            {
                if (CurrentPage < VisibleNumbers)
                {
                    pagerSegments.Add(new PagerSegment(1, VisibleNumbers));
                    pagerSegments.Add(new PagerSegment(TotalPages, TotalPages));
                }
                else if (CurrentPage > TotalPages - VisibleNumbers)
                {
                    if (TotalPages - VisibleNumbers > 1)
                    {
                        pagerSegments.Add(new PagerSegment(1, 1));
                    }
                    pagerSegments.Add(new PagerSegment(TotalPages - VisibleNumbers, TotalPages));
                }
                else
                {
                    if (CurrentPage - VisibleNumbers / 2 > 1)
                    {
                        pagerSegments.Add(new PagerSegment(1, 1));
                    }
                    pagerSegments.Add(new PagerSegment(CurrentPage - VisibleNumbers / 2, CurrentPage + VisibleNumbers / 2));
                    pagerSegments.Add(new PagerSegment(TotalPages, TotalPages));
                }
            }

            for (var idx = 0; idx < pagerSegments.Count; idx++)
            {
                var pagerSegment = pagerSegments[idx];
                for (var i = pagerSegment.StartPageIndex; i <= pagerSegment.EndPageIndex; i++)
                {
                    //var spanTagBuilder = new TagBuilder("span");
                    liTag = new TagBuilder("li");
                    if (i == CurrentPage)
                    {
                        liTag.AddCssClass("active");
                    }
                    liTag.AddCssClass("pn");
                    var linkBuilder = new TagBuilder("a");

                    routeValues[ParameterName] = i;
                    linkBuilder.Attributes.Add("href",
                        string.IsNullOrEmpty(AnchorTagName)
                            ? urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller, Values = routeValues })
                            : string.Format("{0}#{1}", urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller, Values = routeValues }),
                                AnchorTagName));
                    linkBuilder.InnerHtml.Append(i.ToString());
                    liTag.InnerHtml.AppendHtml(linkBuilder.ToHtmlString());
                    ulTag.InnerHtml.AppendHtml(liTag.ToHtmlString());
                }
                if (idx != pagerSegments.Count - 1 &&
                    (pagerSegments[idx].EndPageIndex + 1 != pagerSegments[idx + 1].StartPageIndex))
                {
                    liTag = new TagBuilder("li");
                    liTag.AddCssClass("pn");
                    var aTag = new TagBuilder("a");
                    aTag.InnerHtml.Append("......");
                    liTag.AddCssClass("disabled");
                    liTag.InnerHtml.AppendHtml(aTag.ToHtmlString());
                    ulTag.InnerHtml.AppendHtml(liTag.ToHtmlString());
                }
            }

            TagBuilder nextPageTagBuilder;
            TagBuilder lastPageTagBuilder;
            if (CurrentPage < TotalPages)
            {
                nextPageTagBuilder = new TagBuilder("a");
                nextPageTagBuilder.InnerHtml.Append("下一页");
                routeValues[ParameterName] = CurrentPage + 1;
                nextPageTagBuilder.Attributes.Add("href",
                    string.IsNullOrEmpty(AnchorTagName)
                        ? urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller, Values = routeValues })
                        : string.Format("{0}#{1}", urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller, Values = routeValues }), AnchorTagName));

                liTag = new TagBuilder("li");
                liTag.InnerHtml.AppendHtml(nextPageTagBuilder.ToHtmlString());
                ulTag.InnerHtml.AppendHtml(liTag.ToHtmlString());

                lastPageTagBuilder = new TagBuilder("a");
                lastPageTagBuilder.InnerHtml.Append("末页");
                routeValues[ParameterName] = TotalPages;
                lastPageTagBuilder.Attributes.Add("href",
                    string.IsNullOrEmpty(AnchorTagName)
                        ? urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller, Values = routeValues })
                        : string.Format("{0}#{1}", urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller, Values = routeValues }), AnchorTagName));
                liTag = new TagBuilder("li");
                liTag.InnerHtml.AppendHtml(lastPageTagBuilder.ToHtmlString());
                ulTag.InnerHtml.AppendHtml(liTag.ToHtmlString());
            }
            else
            {
                nextPageTagBuilder = new TagBuilder("a");
                nextPageTagBuilder.InnerHtml.Append("下一页");
                liTag = new TagBuilder("li");
                liTag.AddCssClass("disabled");
                liTag.InnerHtml.AppendHtml(nextPageTagBuilder.ToHtmlString());
                ulTag.InnerHtml.AppendHtml(liTag.ToHtmlString());

                lastPageTagBuilder = new TagBuilder("a");
                lastPageTagBuilder.InnerHtml.Append("末页");
                liTag = new TagBuilder("li");
                liTag.AddCssClass("disabled");
                liTag.InnerHtml.AppendHtml(lastPageTagBuilder.ToHtmlString());
                ulTag.InnerHtml.AppendHtml(liTag.ToHtmlString());
            }

            divBuilder.InnerHtml.AppendHtml(ulTag.ToHtmlString());
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(divBuilder.ToHtmlString());
        }
    }
}
