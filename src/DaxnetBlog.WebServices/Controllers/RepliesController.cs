using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DaxnetBlog.Common.Storage;
using DaxnetBlog.Domain.Model;

namespace DaxnetBlog.WebServices.Controllers
{
    [Route("api/[controller]")]
    public class RepliesController : Controller
    {
        private readonly IStorage storage;
        private readonly IEntityStore<Reply, int> replyStore;

        public RepliesController(IStorage storage, IEntityStore<Reply, int> replyStore)
        {
            this.storage = storage;
            this.replyStore = replyStore;
        }

        [HttpGet]
        [Route("post/{postId}")]
        public async Task<IActionResult> GetByPostId(int postId)
        {
            IEnumerable<Reply> replies = await this.storage.ExecuteAsync(async (connection, cancellationToken) =>
                await replyStore.SelectAsync(connection,
                    x => x.BlogPostId == postId,
                    new Sort<Reply, int> { { x => x.DatePublished, SortOrder.Ascending } },
                    cancellationToken: cancellationToken));
            throw new NotImplementedException();
        }
    }
}
