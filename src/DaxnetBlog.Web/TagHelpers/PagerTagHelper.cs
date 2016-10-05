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
            divBuilder.AddCssClass("pager");
            var pageNumberStringBuilder = new StringBuilder();
            var urlHelper = this.UrlHelperFactory.GetUrlHelper(this.ViewContext);
            TagBuilder firstPageTagBuilder;
            TagBuilder prevPageTagBuilder;
            if (this.CurrentPage > 1)
            {
                firstPageTagBuilder = new TagBuilder("a");
                firstPageTagBuilder.InnerHtml.Append("首页");
                firstPageTagBuilder.AddCssClass("quick-nav");
                firstPageTagBuilder.Attributes.Add("href",
                    string.IsNullOrEmpty(AnchorTagName)
                        ? urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller })
                        : string.Format("{0}#{1}", urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller }), AnchorTagName));

                pageNumberStringBuilder.Append(firstPageTagBuilder.ToHtmlString());

                prevPageTagBuilder = new TagBuilder("a");
                prevPageTagBuilder.InnerHtml.Append("上一页");
                prevPageTagBuilder.AddCssClass("quick-nav");
                routeValues[ParameterName] = CurrentPage - 1;
                prevPageTagBuilder.Attributes.Add("href",
                    string.IsNullOrEmpty(AnchorTagName)
                        ? urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller, Values = routeValues })
                        : string.Format("{0}#{1}", urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller, Values = routeValues }), AnchorTagName));

                pageNumberStringBuilder.Append(prevPageTagBuilder.ToHtmlString());
            }
            else
            {
                firstPageTagBuilder = new TagBuilder("span");
                firstPageTagBuilder.InnerHtml.Append("首页");
                firstPageTagBuilder.AddCssClass("quick-nav-disabled");
                pageNumberStringBuilder.Append(firstPageTagBuilder.ToHtmlString());

                prevPageTagBuilder = new TagBuilder("span");
                prevPageTagBuilder.InnerHtml.Append("上一页");
                prevPageTagBuilder.AddCssClass("quick-nav-disabled");
                pageNumberStringBuilder.Append(prevPageTagBuilder.ToHtmlString());
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
                    var spanTagBuilder = new TagBuilder("span");
                    if (i == CurrentPage)
                    {
                        spanTagBuilder.AddCssClass("current");
                    }
                    spanTagBuilder.InnerHtml.Append(i.ToString());

                    var linkBuilder = new TagBuilder("a");

                    routeValues[ParameterName] = i;
                    linkBuilder.Attributes.Add("href",
                        string.IsNullOrEmpty(AnchorTagName)
                            ? urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller, Values = routeValues })
                            : string.Format("{0}#{1}", urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller, Values = routeValues }),
                                AnchorTagName));

                    linkBuilder.InnerHtml.AppendHtml(spanTagBuilder.ToHtmlString());

                    pageNumberStringBuilder.Append(linkBuilder.ToHtmlString());
                }
                if (idx != pagerSegments.Count - 1 &&
                    (pagerSegments[idx].EndPageIndex + 1 != pagerSegments[idx + 1].StartPageIndex))
                {
                    pageNumberStringBuilder.Append("......");
                }
            }

            TagBuilder nextPageTagBuilder;
            TagBuilder lastPageTagBuilder;
            if (CurrentPage < TotalPages)
            {
                nextPageTagBuilder = new TagBuilder("a");
                nextPageTagBuilder.InnerHtml.Append("下一页");
                nextPageTagBuilder.AddCssClass("quick-nav");
                routeValues[ParameterName] = CurrentPage + 1;
                nextPageTagBuilder.Attributes.Add("href",
                    string.IsNullOrEmpty(AnchorTagName)
                        ? urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller, Values = routeValues })
                        : string.Format("{0}#{1}", urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller, Values = routeValues }), AnchorTagName));

                pageNumberStringBuilder.Append(nextPageTagBuilder.ToHtmlString());

                lastPageTagBuilder = new TagBuilder("a");
                lastPageTagBuilder.InnerHtml.Append("末页");
                lastPageTagBuilder.AddCssClass("quick-nav");
                routeValues[ParameterName] = TotalPages;
                lastPageTagBuilder.Attributes.Add("href",
                    string.IsNullOrEmpty(AnchorTagName)
                        ? urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller, Values = routeValues })
                        : string.Format("{0}#{1}", urlHelper.Action(new UrlActionContext { Action = this.Action, Controller = this.Controller, Values = routeValues }), AnchorTagName));
                pageNumberStringBuilder.Append(lastPageTagBuilder.ToHtmlString());
            }
            else
            {
                nextPageTagBuilder = new TagBuilder("span");
                nextPageTagBuilder.InnerHtml.Append("下一页");
                nextPageTagBuilder.AddCssClass("quick-nav-disabled");
                pageNumberStringBuilder.Append(nextPageTagBuilder.ToHtmlString());

                lastPageTagBuilder = new TagBuilder("span");
                lastPageTagBuilder.InnerHtml.Append("末页");
                lastPageTagBuilder.AddCssClass("quick-nav-disabled");
                pageNumberStringBuilder.Append(lastPageTagBuilder.ToHtmlString());
            }



            divBuilder.InnerHtml.AppendHtml(pageNumberStringBuilder.ToString());
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(divBuilder.ToHtmlString());
        }
    }
}
