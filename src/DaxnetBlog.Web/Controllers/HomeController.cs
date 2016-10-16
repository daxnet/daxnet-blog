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

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="config">The configuration.</param>
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

        public IActionResult Error()
        {
            return View();
        }

        #region Helper Actions        
        /// <summary>
        /// Updates the captcha and return the HTML string representing the
        /// updated captcha.
        /// </summary>
        /// <returns></returns>
        public IActionResult UpdateCaptcha()
        {
            return PartialView("_CaptchaPartial");
        }

        /// <summary>
        /// Verifies the captcha.
        /// </summary>
        /// <param name="captchaString">The captcha string.</param>
        /// <param name="encryptedString">The encrypted string.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult VerifyCaptcha(string captchaString, string encryptedString)
        {
            var enc = Convert.ToBase64String(UTF32Encoding.Unicode.GetBytes(captchaString));
            return Ok(enc == encryptedString);
        }

        /// <summary>
        /// Posts the reply comments.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="key">The key.</param>
        /// <param name="comments">The comments.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Reply(string feature, string key, string comments)
        {
            var userName = this.User.Identity.Name;
            //ViewData["Message"] = "处理成功，您的回复已经提交并进入审核流程。";
            return Ok(userName);
            //throw new Exception("失败");
        }
        #endregion
    }
}
