using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
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

        #region Ping API
        [Route("ping")]
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok(new {
                Result = true
            });
        }
        #endregion

        #region Reply API
        [Route("replies/all")]
        [HttpGet]
        public async Task<IActionResult> GetAllReplies()
        {
            var result = await this.httpClient.GetAsync("replies/all");
            result.EnsureSuccessStatusCode();
            dynamic model = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync());
            var replies = new List<object>();
            foreach(dynamic reply in model)
            {
                replies.Add(new
                {
                    reply.reply.id,
                    reply.reply.datePublished,
                    reply.reply.status,
                    reply.reply.account.userName,
                    reply.blogPost.title
                });
            }
            return Ok(replies);
        }

        [Route("replies/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetReplyById(int id)
        {
            var result = await this.httpClient.GetAsync($"replies/{id}");
            result.EnsureSuccessStatusCode();
            dynamic model = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync());
            return Ok(model);
        }

        [Route("replies/approveOrReject/{replyId}")]
        [HttpPost]
        public async Task<IActionResult> ApproveReply(int replyId, [FromBody] dynamic model)
        {
            var operation = (string)model.Operation;
            var result = await this.httpClient.PostAsJsonAsync($"replies/approveOrReject/{replyId}", new { Operation = operation });
            result.EnsureSuccessStatusCode();
            dynamic returnModel = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync());
            return Ok(returnModel);
        }
        #endregion
    }
}
