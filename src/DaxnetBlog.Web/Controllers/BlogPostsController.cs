// ===========================================================================================================
//      _                                 _              _       _                 
//     | |                               | |            | |     | |                
//   __| |   __ _  __  __  _ __     ___  | |_   ______  | |__   | |   ___     __ _ 
//  / _` |  / _` | \ \/ / | '_ \   / _ \ | __| |______| | '_ \  | |  / _ \   / _` |
// | (_| | | (_| |  >  <  | | | | |  __/ | |_           | |_) | | | | (_) | | (_| |
//  \__,_|  \__,_| /_/\_\ |_| |_|  \___|  \__|          |_.__/  |_|  \___/   \__, |
//                                                                            __/ |
//                                                                           |___/ 
//
// 
// Daxnet Personal Blog
// Copyright © 2016 by daxnet (Sunny Chen)
//
// https://github.com/daxnet/daxnet-blog
//
// MIT License
// 
// Copyright(c) 2016 Sunny Chen
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ===========================================================================================================

using DaxnetBlog.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DaxnetBlog.Web.Controllers
{
    public class BlogPostsController : Controller
    {
        private const string WeblogTitle = "daxnet.me";
        private const string ItemCategoryName = "所有分类";
        private const int WeblogImageWidth = 125;
        private const int WeblogImageHeight = 100;

        private static Uri WeblogUri = new Uri("http://daxnet.me/");
        private static Uri WeblogImageUri = new Uri("https://daxnetblogstorage.blob.core.windows.net/files/sunnychen.png");
        private static CultureInfo WeblogLanguage = new CultureInfo("zh-CN");
        private static CultureInfo RssLocFormat = new CultureInfo("zh-CN");

        private readonly HttpClient httpClient;
        private readonly IOptions<WebsiteSettings> config;
        private readonly ILogger<BlogPostsController> logger;
        private readonly int pageSize;

        public BlogPostsController(HttpClient httpClient,
            IOptions<WebsiteSettings> config,
            ILogger<BlogPostsController> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            this.config = config;
            this.pageSize = this.config.Value.BlogPostsPageSize;
        }

        public async Task<IActionResult> Post(int id)
        {
            var json = await(await this.httpClient.GetAsync($"blogPosts/{id}")).Content.ReadAsStringAsync();
            dynamic model = JsonConvert.DeserializeObject(json);

            if (!Request.Cookies.ContainsKey("X-DXBLOG-POSTIDS"))
            {
                try
                {
                    Response.Cookies.Append("X-DXBLOG-POSTIDS", id.ToString(), new CookieOptions { Path = Request.Path, Expires = DateTimeOffset.Now.AddDays(1) });
                    var visits = (int?)model.visits;
                    var updatedVisits = visits != null && visits.HasValue ? visits.Value + 1 : 1;
                    var updateResult = await this.httpClient.PutAsJsonAsync($"blogPosts/update/{id}", new { Visits = updatedVisits });
                    updateResult.EnsureSuccessStatusCode();
                    model.visits = updatedVisits;
                }
                catch(Exception ex)
                {
                    logger.LogWarning("无法更新博客文章的访问量。", ex.ToString());
                }
            }

            return View(model);
        }

        /// <summary>
        /// Posts the reply comments.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="key">The key.</param>
        /// <param name="comments">The comments.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Reply(string feature, string key, string comments)
        {
            var blogPostId = Convert.ToInt32(key);
            var userName = this.User.Identity.Name;
            var result = await this.httpClient.PostAsJsonAsync($"replies/create", new { BlogPostId = blogPostId, UserName = userName, Content = comments });
            if (result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return Ok();
            }

            var responseText = await result.Content.ReadAsStringAsync();
            return StatusCode((int)HttpStatusCode.InternalServerError, responseText);
        }

        [HttpGet]
        [Route("blogposts/archive/{year}/{month}")]
        public async Task<IActionResult> Archive(int year, int month, int page = 1)
        {
            var json = await(await this.httpClient.GetAsync($"blogPosts/archive/{year}/{month}/{pageSize}/{page}")).Content.ReadAsStringAsync();
            dynamic model = JsonConvert.DeserializeObject(json);
            ViewData["Year"] = year;
            ViewData["Month"] = month;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Rss()
        {
            rss rss = new rss();
            RssChannel rssChannel = new RssChannel();
            rss.channel = rssChannel;

            Image feedImage = new Image();
            feedImage.link = WeblogUri.AbsoluteUri;
            feedImage.title = WeblogTitle;
            feedImage.width = WeblogImageWidth.ToString(RssLocFormat);
            feedImage.height = WeblogImageHeight.ToString(RssLocFormat);
            feedImage.url = WeblogImageUri.AbsoluteUri;

            rssChannel.ItemsElementName = new ItemsChoiceType[] {
                // Ordering must correspond to the contents of rssChannel.Items
                ItemsChoiceType.title,
                ItemsChoiceType.link,
                ItemsChoiceType.language,
                ItemsChoiceType.image
            };

            rssChannel.Items = new object[] {
                WeblogTitle,
                WeblogUri.AbsoluteUri,  // Must be a string value
                WeblogLanguage.ToString(), // Must be a string value
                feedImage
            };

            // Gets all the blog post items from repo.
            var json = await (await this.httpClient.GetAsync($"blogPosts/paginate/{this.pageSize}/1")).Content.ReadAsStringAsync();
            dynamic blogPosts = JsonConvert.DeserializeObject(json);

            var rssItemList = new List<RssItem>();

            foreach (dynamic post in blogPosts.data)
            {
                var rssItem = new RssItem();
                rssItem.ItemsElementName = new ItemsChoiceType1[]
                {
                    ItemsChoiceType1.link,
                    ItemsChoiceType1.pubDate,
                    ItemsChoiceType1.guid,
                    ItemsChoiceType1.title,
                    ItemsChoiceType1.description,
                    ItemsChoiceType1.category
                };

                var itemGuid = new RssItemGuid
                {
                    isPermaLink = false,
                    Value = ((int)post.id).ToString()
                };

                
                var link = Url.Action("Post", "BlogPosts", new { id = (int)post.id }, Url.ActionContext.HttpContext.Request.Scheme);
                var itemUri = new Uri(link);
                var itemPubDate = (DateTime)post.datePublished;
                var itemTitle = (string)post.title;
                var itemBody = (string)post.content;
                var itemCategory = new Category { Value = ItemCategoryName };

                rssItem.Items = new object[] {
                    itemUri.AbsoluteUri,  // Must be a string value
                    itemPubDate.ToString(  // Format data value
                        DateTimeFormatInfo.InvariantInfo.RFC1123Pattern,
                        RssLocFormat ),
                    itemGuid,
                    itemTitle,
                    itemBody,
                    itemCategory
                };

                rssItemList.Add(rssItem);
            }

            rssChannel.item = rssItemList.ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(rss));

            using (var ms = new MemoryStream())
            {
                serializer.Serialize(ms, rss);
                return Content(Encoding.UTF8.GetString(ms.ToArray()), "application/xml");
            }
        }
    }
}
