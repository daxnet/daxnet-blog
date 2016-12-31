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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DaxnetBlog.Common.Storage;
using DaxnetBlog.Domain.Model;
using System.Net;
using System.Linq.Expressions;
using DaxnetBlog.WebServices.Exceptions;
using DaxnetBlog.Common.IntegrationServices;
using DaxnetBlog.WebServices.Caching;

namespace DaxnetBlog.WebServices.Controllers
{
    [Route("api/[controller]")]
    public class RepliesController : Controller
    {
        private readonly IStorage storage;
        private readonly ICachingService cachingService;
        private readonly IEntityStore<Account, int> accountStore;
        private readonly IEntityStore<Reply, int> replyStore;
        private readonly IEntityStore<BlogPost, int> blogPostStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepliesController"/> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="replyStore">The reply store.</param>
        /// <param name="accountStore">The account store.</param>
        public RepliesController(IStorage storage,
            ICachingService cachingService,
            IEntityStore<Reply, int> replyStore,
            IEntityStore<Account, int> accountStore,
            IEntityStore<BlogPost, int> blogPostStore)
        {
            this.storage = storage;
            this.cachingService = cachingService;
            this.replyStore = replyStore;
            this.accountStore = accountStore;
            this.blogPostStore = blogPostStore;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reply = await this.storage.ExecuteAsync(async (connection, transaction, cancellationToken) =>
            {
                var r = (await replyStore.SelectAsync(connection, x => x.Account, x => x.AccountId, acct => acct.Id, x => x.Id == id, transaction: transaction, cancellationToken: cancellationToken)).FirstOrDefault();
                if (r == null)
                {
                    throw new ServiceException(HttpStatusCode.NotFound, Reason.EntityNotFound, $"Id为{id}的回复内容不存在。");
                }
                return r;
            });

            return Ok(new
            {
                reply.Id,
                reply.BlogPostId,
                reply.Content,
                reply.DatePublished,
                reply.ParentId,
                reply.Status,
                Account = new
                {
                    reply.Account.Id,
                    reply.Account.UserName,
                    reply.Account.NickName
                }
            });
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
                throw new ServiceException(HttpStatusCode.BadRequest, Reason.ArgumentNull, $"{nameof(userName)}不能为空。");
            }

            if (string.IsNullOrEmpty(content))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, Reason.ArgumentNull, $"{nameof(content)}不能为空。");
            }

            var replyId = await this.storage.ExecuteAsync(async (connection, transaction, cancellationToken) =>
               {
                   var user = (await accountStore.SelectAsync(connection, 
                       x => x.UserName == userName, 
                       transaction: transaction, 
                       cancellationToken: cancellationToken)).FirstOrDefault();

                   if (user == null)
                   {
                       throw new ServiceException(HttpStatusCode.NotFound, Reason.EntityNotFound, $"用户 \"{userName}\" 不存在。");
                   }
                   var userId = user.Id;
                   var reply = new Reply
                   {
                       AccountId = userId,
                       BlogPostId = blogPostId,
                       DatePublished = DateTime.UtcNow,
                       Status = ReplyStatus.Created,
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

            if (replyId > 0)
            {
                var key = new CachingKey(CachingKeys.BLOGPOSTS_POST_KEY, blogPostId);
                this.cachingService.Delete(key);

                var uri = Url.Action("GetById", new { id = replyId });
                return Created(uri, replyId);
            }
            throw new ServiceException(Reason.CreateFailed, "创建回复失败。");
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllReplies()
        {
            var result = await this.storage.ExecuteAsync(async (connection, transaction, cancellationToken) =>
            {
                var replies = await this.replyStore.SelectAsync(connection, r => r.Account,
                    r => r.AccountId, acct => acct.Id,
                    sorting: new Sort<Reply, int> { { x => x.DatePublished, SortOrder.Descending } }, 
                    transaction: transaction, 
                    cancellationToken: cancellationToken);

                // TODO: Note that for now it is just a workaround for retrieving the data for another
                // associated object on the current one. The limitation of the current entity store implementation
                // is that if only one associated object is going to be retrieved along with the current object,
                // the SelectAsync operation can do the join. However if multiple associated objects are going to
                // be retrieved, the join cannot be performed. For current scenario, both account information and
                // blog post information needs to be read along with the reply data, so, for account information,
                // we performed the inline join, but for the blog post information, we issued another database
                // connection.
                var list = new List<object>();
                foreach(var reply in replies)
                {
                    var blogPostId = reply.BlogPostId;
                    var blogPost = (await this.blogPostStore.SelectAsync(connection, bp => bp.Id == blogPostId,
                        transaction: transaction,
                        cancellationToken: cancellationToken)).FirstOrDefault();
                    list.Add(new
                    {
                        Reply = reply,
                        BlogPost = blogPost
                    });
                }

                return list;
                
            });

            return Ok(result);
        }


        [HttpPost]
        [Route("approveOrReject/{replyId}")]
        public async Task<IActionResult> ApproveOrReject(int replyId, [FromBody] dynamic model)
        {
            var operation = (string)model.Operation;
            if (string.IsNullOrEmpty(operation))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, Reason.InvalidArgument, "回复审批操作方式未指定，请指定Approve或者Reject操作。");
            }

            if (operation.ToUpper() != "APPROVE" &&
                operation.ToUpper() != "REJECT")
            {
                throw new ServiceException(HttpStatusCode.MethodNotAllowed, Reason.InvalidArgument, "指定的审批操作方式不可用，请指定Approve或者Reject操作。");
            }

            var blogPostId = -1;
            var affectedRows = await this.storage.ExecuteAsync(async (connection, transaction, cancellationToken) =>
            {
                var reply = (await this.replyStore.SelectAsync(connection,
                    r => r.Id == replyId,
                    transaction: transaction,
                    cancellationToken: cancellationToken)).FirstOrDefault();
                if (reply == null)
                {
                    throw new ServiceException(HttpStatusCode.NotFound, Reason.EntityNotFound, $"ID为{replyId}的用户回复内容不存在。");
                }
                blogPostId = reply.BlogPostId;
                var updateFields = new List<Expression<Func<Reply, object>>>();
                switch (operation.ToUpper())
                {
                    case "APPROVE":
                        reply.Status = ReplyStatus.Approved;
                        break;
                    case "REJECT":
                        reply.Status = ReplyStatus.Rejected;
                        break;
                }

                return await this.replyStore.UpdateAsync(reply,
                    connection,
                    r => r.Id == replyId,
                    new Expression<Func<Reply, object>>[] { r => r.Status },
                    transaction,
                    cancellationToken);
            });

            if (affectedRows > 0)
            {
                if (blogPostId >=0)
                {
                    var key = new CachingKey(CachingKeys.BLOGPOSTS_POST_KEY, blogPostId);
                    this.cachingService.Delete(key);
                }

                return Ok(affectedRows);
            }

            throw new ServiceException(Reason.UpdateFailed, "用户回复审批失败。");
        }
    }
}
