using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DaxnetBlog.Domain.Model;
using DaxnetBlog.Common.Storage;
using System.Net;
using System.Linq.Expressions;

namespace DaxnetBlog.WebServices.Controllers
{
    [Route("api/[controller]")]
    public class BlogPostsController : Controller
    {
        private readonly IStorage storage;
        private readonly IEntityStore<BlogPost, int> blogPostStore;
        private readonly IEntityStore<Reply, int> replyStore;

        public BlogPostsController(IStorage storage, 
            IEntityStore<BlogPost, int> blogPostStore,
            IEntityStore<Reply, int> replyStore)
        {
            this.storage = storage;
            this.blogPostStore = blogPostStore;
            this.replyStore = replyStore;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] dynamic model)
        {
            var title = (string)model.Title;
            var content = (string)model.Content;
            var accountId = (int)model.AccountId;

            if (string.IsNullOrEmpty(title))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, "博客日志标题不能为空。");
            }

            if (string.IsNullOrEmpty(content))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, "博客日志内容不能为空。");
            }

            if (accountId <= 0)
            {
                throw new ServiceException(HttpStatusCode.BadRequest, "用户账户Id值不在有效范围。");
            }

            var result = await this.storage.ExecuteAsync(async (connection, transaction, cancellationToken) =>
            {
                var blogPost = new BlogPost
                {
                    AccountId = accountId,
                    DatePublished = DateTime.UtcNow,
                    Content = content,
                    IsDeleted = false,
                    Title = title
                };

                var affectedRows = await this.blogPostStore.InsertAsync(blogPost,
                    connection, new Expression<Func<BlogPost, object>>[] { bp => bp.Id },
                    transaction, cancellationToken);

                if (affectedRows > 0)
                {
                    var lastInsertedBlogPost = (await this.blogPostStore.SelectAsync(connection,
                        sorting: new Sort<BlogPost, int> { { bp => bp.DatePublished, SortOrder.Descending } }, transaction: transaction,
                        cancellationToken: cancellationToken)).FirstOrDefault();
                    if (lastInsertedBlogPost != null)
                    {
                        return lastInsertedBlogPost.Id;
                    }
                }

                return 0;
            });

            if (result != 0)
            {
                return Created(Url.Action("GetById", new { id = result }), result);
            }

            return NoContent();
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await this.storage.ExecuteAsync(async (connection, transaction, cancellationToken) =>
            {
                var blogPost = (await this.blogPostStore.SelectAsync(connection,
                    bp => bp.Id == id,
                    transaction: transaction,
                    cancellationToken: cancellationToken)).FirstOrDefault();
                if (blogPost == null)
                {
                    throw new ServiceException(HttpStatusCode.NotFound, $"Id为{id}的博客日志不存在。");
                }

                blogPost.IsDeleted = true;
                return await this.blogPostStore.UpdateAsync(blogPost,
                    connection,
                    p => p.Id == id,
                    new Expression<Func<BlogPost, object>>[] { bp => bp.IsDeleted },
                    transaction, cancellationToken);
            });

            if (result > 0)
                return Ok();
            throw new ServiceException("删除博客日志失败。");
        }

        [HttpPut]
        [Route("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] dynamic model)
        {
            var title = (string)model.Title;
            var content = (string)model.Content;
            var datePublished = (DateTime?)model.DatePublished;

            var result = await this.storage.ExecuteAsync(async (connection, transaction, cancellationToken) =>
            {
                var blogPost = (await this.blogPostStore.SelectAsync(connection,
                    bp => bp.Id == id,
                    transaction: transaction,
                    cancellationToken: cancellationToken)).FirstOrDefault();
                if (blogPost == null)
                {
                    throw new ServiceException(HttpStatusCode.NotFound, $"Id为{id}的博客日志不存在。");
                }

                var listUpdateFields = new List<Expression<Func<BlogPost, object>>>();
                if (!string.IsNullOrEmpty(title))
                {
                    blogPost.Title = title;
                    listUpdateFields.Add(x => x.Title);
                }

                if (!string.IsNullOrEmpty(content))
                {
                    blogPost.Content = content;
                    listUpdateFields.Add(x => x.Content);
                }

                if (datePublished != null && datePublished.HasValue && datePublished != DateTime.MinValue)
                {
                    blogPost.DatePublished = datePublished.Value;
                    listUpdateFields.Add(x => x.DatePublished);
                }

                return await this.blogPostStore.UpdateAsync(blogPost, connection,
                    x => x.Id == id,
                    listUpdateFields, transaction, cancellationToken);
            });

            if (result > 0)
                return Ok();
            throw new ServiceException("更新博客日志失败。");
        }

        [HttpGet]
        [Route("paginate/{pageSize}/{pageNumber}")]
        public async Task<IActionResult> GetByPaging(int pageSize, int pageNumber)
        {
            var pagedModel = await this.storage.ExecuteAsync(async (connection, cancellationToken) =>
                await blogPostStore.SelectAsync(pageNumber, pageSize, connection, new Sort<BlogPost, int> { { x=>x.DatePublished, SortOrder.Descending } },
                    expr => expr.IsDeleted == null || expr.IsDeleted.Value == false)
            );

            return Ok(new
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalRecords = pagedModel.TotalRecords,
                TotalPages = pagedModel.TotalPages,
                Count = pagedModel.Count,
                Data = pagedModel.Select(x=>new
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    Title = x.Title,
                    Content = x.Content,
                    DatePublished = x.DatePublished,
                    UpVote = x.UpVote.HasValue ? x.UpVote.Value : 0,
                    DownVote = x.DownVote.HasValue ? x.DownVote.Value : 0,
                    Visits = x.Visits.HasValue ? x.Visits.Value : 0
                })
            });
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await this.storage.ExecuteAsync(async (connection, transaction, cancellationToken) =>
            {
                var blogpost = (await blogPostStore.SelectAsync(connection, p => p.Account, 
                    p=>p.AccountId, 
                    acct=>acct.Id, 
                    x => x.Id == id, 
                    transaction: transaction, 
                    cancellationToken: cancellationToken)).FirstOrDefault();

                if (blogpost == null)
                {
                    throw new ServiceException(HttpStatusCode.NotFound, $"The blog post of Id ${id} doesn't exist.");
                }

                var replies = await replyStore.SelectAsync(connection,
                    r => r.Account,
                    r => r.AccountId,
                    a => a.Id,
                    r => r.BlogPostId == id && r.Status == ReplyStatus.Approved,
                    transaction: transaction, cancellationToken: cancellationToken);

                return new Tuple<BlogPost, IEnumerable<Reply>>(blogpost, replies);
            });

            return Ok(new
            {
                Id = id,
                Title = result.Item1.Title,
                Content = result.Item1.Content,
                DatePublished = result.Item1.DatePublished,
                Replies = result.Item2.Select(x=> new {
                    x.Id,
                    x.DatePublished,
                    x.Content,
                    AccountId = x.Account.Id,
                    x.Account.UserName,
                    x.Account.NickName
                }),
                Account = new
                {
                    result.Item1.Account.Id,
                    result.Item1.Account.UserName,
                    result.Item1.Account.NickName
                }
            });
        }
    }
}
