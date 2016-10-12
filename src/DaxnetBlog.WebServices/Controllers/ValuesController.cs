using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DaxnetBlog.Common.Storage;
using DaxnetBlog.Domain.EntityStore;
using DaxnetBlog.Domain.Model;
using System.Linq.Expressions;
using DaxnetBlog.Common;

namespace DaxnetBlog.WebServices.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IStorage storage;
        private readonly IEntityStore<Reply, int> replyStore;

        public ValuesController(IStorage storage, IEntityStore<Reply, int> replyStore)
        {
            this.storage = storage;
            this.replyStore = replyStore;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //PagedResult<Account, int> paged = null;
            //this.storage.Execute(connection =>
            //{
            //    paged = accountStore.Select(4, 3, connection, new Sort<Account, int> { { x => x.DateRegistered, SortOrder.Descending } });
            //});
            //var replies = await this.storage.ExecuteAsync(async (connection, cancellationToken) => 
            //    await replyStore.SelectAsync(connection, x => x.Account, x => x.AccountId, y => y.Id/*, x => x.BlogPostId == 1*/));
            //return Ok(replies);
            return await Task.FromResult(Ok(new[] { "test" }));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
