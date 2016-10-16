using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DaxnetBlog.Common.Storage;
using DaxnetBlog.Domain.Model;
using System.Net;
using System.Linq.Expressions;

namespace DaxnetBlog.WebServices.Controllers
{
    [Route("api/[controller]")]
    public class RepliesController : Controller
    {
        private readonly IStorage storage;
        private readonly IEntityStore<Account, int> accountStore;
        private readonly IEntityStore<Reply, int> replyStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliesController"/> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="replyStore">The reply store.</param>
        /// <param name="accountStore">The account store.</param>
        public RepliesController(IStorage storage,
            IEntityStore<Reply, int> replyStore,
            IEntityStore<Account, int> accountStore)
        {
            this.storage = storage;
            this.replyStore = replyStore;
            this.accountStore = accountStore;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateReply([FromBody] dynamic model)
        {
            var blogPostId = (int)model.BlogPostId;
            var userName = (string)model.UserName;
            var content = (string)model.Content;

            if (string.IsNullOrEmpty(userName))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, $"{nameof(userName)}不能为空。");
            }

            if (string.IsNullOrEmpty(content))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, $"{nameof(content)}不能为空。");
            }

            var replyId = await this.storage.ExecuteAsync(async (connection, transaction, cancellationToken) =>
               {
                   var user = (await accountStore.SelectAsync(connection, 
                       x => x.UserName == userName, 
                       transaction: transaction, 
                       cancellationToken: cancellationToken)).FirstOrDefault();

                   if (user == null)
                   {
                       throw new ServiceException(HttpStatusCode.NotFound, $"用户 \"{userName}\" 不存在。");
                   }
                   var userId = user.Id;
                   var reply = new Reply
                   {
                       AccountId = userId,
                       BlogPostId = blogPostId,
                       DatePublished = DateTime.UtcNow,
                       IsApproved = false,
                       Content = content
                   };

                   var rowsAffected = await replyStore.InsertAsync(reply, 
                       connection, 
                       new Expression<Func<Reply, object>>[] { x => x.Id }, 
                       transaction, 
                       cancellationToken);

                   if (rowsAffected > 0)
                   {
                       var insertedReply = (await replyStore.SelectAsync(connection,
                           x => x.BlogPostId == blogPostId && x.AccountId == userId,
                           new Sort<Reply, int> { { r => r.DatePublished, SortOrder.Descending } },
                           transaction, cancellationToken)).FirstOrDefault();
                       return insertedReply.Id;
                   }
                   return 0;
               });
            var uri = Url.Action("GetById", new { id = replyId });
            return Created(uri, replyId);
        }

    }
}
