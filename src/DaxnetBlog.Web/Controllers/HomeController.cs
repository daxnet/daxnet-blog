using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.Net.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using DaxnetBlog.Web.Models;
using System.Text;

namespace DaxnetBlog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly IOptions<WebsiteSettings> config;
        private readonly int pageSize;

        public HomeController(HttpClient httpClient,
            IOptions<WebsiteSettings> config)
        {
            this.httpClient = httpClient;
            this.config = config;
            this.pageSize = this.config.Value.BlogPostsPageSize;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var json = await (await this.httpClient.GetAsync($"blogPosts/paginate/{pageSize}/{page}")).Content.ReadAsStringAsync();
            dynamic model = JsonConvert.DeserializeObject(json);
            return View(model);
        }

        public async Task<IActionResult> Post(int id)
        {
            var json = await (await this.httpClient.GetAsync($"blogPosts/{id}")).Content.ReadAsStringAsync();
            dynamic model = JsonConvert.DeserializeObject(json);
            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [HttpPost]
        public IActionResult Reply(string feature, string key, string comments)
        {
            var userName = this.User.Identity.Name;
            return Ok(userName);
        }

        [HttpPost]
        public IActionResult VerifyCaptcha(string captchaString, string encryptedString)
        {
            var enc = Convert.ToBase64String(UTF32Encoding.Unicode.GetBytes(captchaString));
            return Ok(enc == encryptedString);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
