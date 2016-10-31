using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace DaxnetBlog.Web.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly HttpClient httpClient;

        public BlogPostsController(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IActionResult> Post(int id)
        {
            var json = await(await this.httpClient.GetAsync($"blogPosts/{id}")).Content.ReadAsStringAsync();
            dynamic model = JsonConvert.DeserializeObject(json);
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
