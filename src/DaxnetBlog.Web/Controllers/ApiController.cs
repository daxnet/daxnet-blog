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
        public async Task<IActionResult> ApproveOrRejectReply(int replyId, [FromBody] dynamic model)
        {
            var operation = (string)model.Operation;
            var result = await this.httpClient.PostAsJsonAsync($"replies/approveOrReject/{replyId}", new { Operation = operation });
            result.EnsureSuccessStatusCode();
            dynamic returnModel = JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync());
            return Ok(returnModel);
        }
        #endregion

        #region Account API
        [Route("accounts/all")]
        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            var result = await this.httpClient.GetAsync($"accounts/all");
            result.EnsureSuccessStatusCode();
            return Ok(JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync()));
        }
        #endregion
    }
}
