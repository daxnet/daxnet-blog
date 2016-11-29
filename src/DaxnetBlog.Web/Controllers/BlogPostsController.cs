using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DaxnetBlog.Common;

namespace DaxnetBlog.Web.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<BlogPostsController> logger;

        public BlogPostsController(HttpClient httpClient,
            ILogger<BlogPostsController> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
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
    }
}
