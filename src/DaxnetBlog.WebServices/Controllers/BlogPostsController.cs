using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DaxnetBlog.Domain.Model;
using DaxnetBlog.Common.Storage;
using System.Net;
using System.Linq.Expressions;
using DaxnetBlog.WebServices.Exceptions;
using DaxnetBlog.Common.IntegrationServices;
using DaxnetBlog.WebServices.Caching;

namespace DaxnetBlog.WebServices.Controllers
{
    /// <summary>
    /// Represents the RESTful API controller that provides blog post features.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/[controller]")]
    public class BlogPostsController : Controller
    {
        private readonly IStorage storage;
        private readonly ICachingService cachingService;
        private readonly IEntityStore<BlogPost, int> blogPostStore;
        private readonly IEntityStore<Reply, int> replyStore;

        public BlogPostsController(IStorage storage, 
            ICachingService cachingService,
            IEntityStore<BlogPost, int> blogPostStore,
            IEntityStore<Reply, int> replyStore)
        {
            this.storage = storage;
            this.cachingService = cachingService;
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
                throw new ServiceException(HttpStatusCode.BadRequest, Reason.ArgumentNull, "博客日志标题不能为空。");
            }

            if (string.IsNullOrEmpty(content))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, Reason.ArgumentNull, "博客日志内容不能为空。");
            }

            if (accountId <= 0)
            {
                throw new ServiceException(HttpStatusCode.BadRequest, Reason.ArgumentNull, "用户帐号Id值不在有效范围。");
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
                this.cachingService.DeleteByPrefix(CachingKeys.BLOGPOSTS_GETBYPAGING_KEY);
                var archiveListCachingKey = new CachingKey(CachingKeys.BLOGPOSTS_GETARCHIVELIST_KEY);
                this.cachingService.Delete(archiveListCachingKey);
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
                    throw new ServiceException(HttpStatusCode.NotFound, Reason.EntityNotFound, $"Id为{id}的博客日志不存在。");
                }

                blogPost.IsDeleted = true;
                return await this.blogPostStore.UpdateAsync(blogPost,
                    connection,
                    p => p.Id == id,
                    new Expression<Func<BlogPost, object>>[] { bp => bp.IsDeleted },
                    transaction, cancellationToken);
            });

            if (result > 0)
            {
                // Removes the specific post from the cache
                var blogPostCachingKey = new CachingKey(CachingKeys.BLOGPOSTS_POST_KEY, id);
                this.cachingService.Delete(blogPostCachingKey);
                var archiveListCachingKey = new CachingKey(CachingKeys.BLOGPOSTS_GETARCHIVELIST_KEY);
                this.cachingService.Delete(archiveListCachingKey);

                // Removes all the posts with paging information, from the cache
                this.cachingService.DeleteByPrefix(CachingKeys.BLOGPOSTS_GETBYPAGING_KEY);
                return Ok();
            }

            throw new ServiceException(Reason.DeleteFailed, "删除博客日志失败。");
        }

        /// <summary>
        /// Updates the blog post which has the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the blog post to be updated.</param>
        /// <param name="model">The model which contains the updating information.</param>
        /// <returns>
        /// HTTP 200: Update successful.
        /// </returns>
        /// <exception cref="ServiceException">Failed to update the blog post.</exception>
        [HttpPut]
        [Route("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] dynamic model)
        {
            var title = (string)model.Title;
            var content = (string)model.Content;
            var datePublished = (DateTime?)model.DatePublished;
            var visits = (int?)model.Visits;

            var result = await this.storage.ExecuteAsync(async (connection, transaction, cancellationToken) =>
            {
                var blogPost = (await this.blogPostStore.SelectAsync(connection,
                    bp => bp.Id == id,
                    transaction: transaction,
                    cancellationToken: cancellationToken)).FirstOrDefault();
                if (blogPost == null)
                {
                    throw new ServiceException(HttpStatusCode.NotFound, Reason.EntityNotFound, $"Id为{id}的博客日志不存在。");
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

                if (visits != null && visits.HasValue && visits != int.MinValue)
                {
                    blogPost.Visits = visits.Value;
                    listUpdateFields.Add(x => x.Visits);
                }

                return await this.blogPostStore.UpdateAsync(blogPost, connection,
                    x => x.Id == id,
                    listUpdateFields, transaction, cancellationToken);
            });

            if (result > 0)
            {
                // Removes the specific post from the cache
                var cachingKey = new CachingKey(CachingKeys.BLOGPOSTS_POST_KEY, id);
                this.cachingService.Delete(cachingKey);

                return Ok();
            }
            throw new ServiceException(Reason.UpdateFailed, "更新博客日志失败。");
        }

        /// <summary>
        /// Gets the paged result of the blog posts.
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("paginate/{pageSize}/{pageNumber}")]
        public async Task<IActionResult> GetByPaging(int pageSize, int pageNumber)
        {
            var cachingKey = new CachingKey(CachingKeys.BLOGPOSTS_GETBYPAGING_KEY, pageSize, pageNumber);
            var cachedResult = this.cachingService.Get(cachingKey);
            if (cachedResult != null)
            {
                return Ok(cachedResult);
            }

            var pagedModel = await this.storage.ExecuteAsync(async (connection, cancellationToken) =>
                await blogPostStore.SelectAsync(pageNumber, pageSize, connection, new Sort<BlogPost, int> { { x=>x.DatePublished, SortOrder.Descending } },
                    expr => expr.IsDeleted == null || expr.IsDeleted.Value == false)
            );

            var result = new
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalRecords = pagedModel.TotalRecords,
                TotalPages = pagedModel.TotalPages,
                Count = pagedModel.Count,
                Data = pagedModel.Select(x => new
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
            };

            this.cachingService.Put(cachingKey, result);

            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var key = new CachingKey(CachingKeys.BLOGPOSTS_POST_KEY, id);
            var cachedObject = this.cachingService.Get(key);
            if (cachedObject != null)
            {
                return Ok(cachedObject);
            }

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
                    throw new ServiceException(HttpStatusCode.NotFound, Reason.EntityNotFound, $"Id为{id}的博客不存在。");
                }

                var replies = await replyStore.SelectAsync(connection,
                    r => r.Account,
                    r => r.AccountId,
                    a => a.Id,
                    r => r.BlogPostId == id && r.Status == ReplyStatus.Approved,
                    transaction: transaction, cancellationToken: cancellationToken);

                return new Tuple<BlogPost, IEnumerable<Reply>>(blogpost, replies);
            });

            var ret = new
            {
                Id = id,
                Title = result.Item1.Title,
                Content = result.Item1.Content,
                DatePublished = result.Item1.DatePublished,
                Visits = result.Item1.Visits,
                Replies = result.Item2.Select(x => new
                {
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
            };

            this.cachingService.Put(key, ret);

            return Ok(ret);
        }

        [HttpGet]
        [Route("archive/list")]
        public async Task<IActionResult> GetArchiveList()
        {
            var key = new CachingKey(CachingKeys.BLOGPOSTS_GETARCHIVELIST_KEY);
            var archiveList = this.cachingService.Get(key);
            if (archiveList == null)
            {
                var result = await this.storage.ExecuteAsync(async (connection, cancellationToken) =>
                    await this.blogPostStore.SelectAsync(connection, cancellationToken: cancellationToken));

                var allDates = result.Select(post => post.DatePublished);
                var query = from date in allDates
                            orderby date descending
                            select new { Year = date.Year, Month = date.Month };

                var dict = new Dictionary<string, Tuple<int, int, int>>();
                foreach(var item in query)
                {
                    var k = string.Format("{0:D4}年{1:D2}月", item.Year, item.Month);
                    if (dict.ContainsKey(k))
                    {
                        dict[k] = new Tuple<int, int, int>(item.Year, item.Month, dict[k].Item3 + 1);
                    }
                    else
                    {
                        dict.Add(k, new Tuple<int, int, int>(item.Year, item.Month, 1));
                    }
                }
                var lst = new List<object>();
                foreach (var kvp in dict)
                {
                    lst.Add(new { Text = kvp.Key, Year = kvp.Value.Item1, Month = kvp.Value.Item2, Count = kvp.Value.Item3 });
                }
                archiveList = lst;
                this.cachingService.Put(key, archiveList);
            }

            return Ok(archiveList);
        }

        [HttpGet]
        [Route("archive/{year}/{month}/{pageSize}/{pageNumber}")]
        public async Task<IActionResult> GetArchivedPostsForMonth(int year, int month, int pageSize, int pageNumber)
        {
            var startDate = new DateTime(year, month, 1).ToUniversalTime();
            var endDate = DateTime.MinValue;
            if (month ==12)
            {
                endDate = new DateTime(year, 12, 31).ToUniversalTime();
            }
            else
            {
                endDate = new DateTime(year, month + 1, 1).AddDays(-1).ToUniversalTime();
            }

            var pagedModel = await this.storage.ExecuteAsync(async (connection, cancellationToken) =>
                await blogPostStore.SelectAsync(pageNumber, pageSize, connection, new Sort<BlogPost, int> { { x => x.DatePublished, SortOrder.Descending } },
                    expr => (expr.IsDeleted == null || expr.IsDeleted.Value == false) && expr.DatePublished >= startDate && expr.DatePublished <= endDate)
            );

            var result = new
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalRecords = pagedModel.TotalRecords,
                TotalPages = pagedModel.TotalPages,
                Count = pagedModel.Count,
                Data = pagedModel.Select(x => new
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
            };

            return Ok(result);
        }
    }
}
