using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace DaxnetBlog.Web.Controllers
{
    [Route("api/management")]
    [Authorize("Administration")]
    public class ApiController : Controller
    {
        private readonly HttpClient httpClient;

        public ApiController(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        [Route("ping")]
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok(new {
                Result = true
            });
        }

        [Route("replies/all")]
        [HttpGet]
        public async Task<IActionResult> GetAllReplies()
        {
            var result = await this.httpClient.GetAsync("replies/all");
            result.EnsureSuccessStatusCode();
            var model = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync());
            return Ok(model);
        }
    }
}
