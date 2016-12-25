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
using Microsoft.Extensions.Logging;

namespace DaxnetBlog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly IOptions<WebsiteSettings> config;
        private readonly int pageSize;
        private readonly ILogger<HomeController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="config">The configuration.</param>
        public HomeController(HttpClient httpClient,
            IOptions<WebsiteSettings> config,
            ILogger<HomeController> logger)
        {
            this.httpClient = httpClient;
            this.config = config;
            this.pageSize = this.config.Value.BlogPostsPageSize;
            this.logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, int year = -1, int month = -1)
        {
            this.logger.LogInformation("home page requested.");
            var json = string.Empty;
            if (year == -1 && month == -1)
            {
                json = await (await this.httpClient.GetAsync($"blogPosts/paginate/{pageSize}/{page}")).Content.ReadAsStringAsync();
            }
            
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
    }
}
