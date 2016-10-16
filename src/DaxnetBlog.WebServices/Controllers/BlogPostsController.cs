using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DaxnetBlog.Domain.Model;
using DaxnetBlog.Common.Storage;
using System.Net;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

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

        [HttpGet]
        [Route("paginate/{pageSize}/{pageNumber}")]
        public async Task<IActionResult> GetByPaging(int pageSize, int pageNumber)
        {
            var pagedModel = await this.storage.ExecuteAsync(async (connection, cancellationToken) =>
                await blogPostStore.SelectAsync(pageNumber, pageSize, connection, new Sort<BlogPost, int> { { x=>x.DatePublished, SortOrder.Descending } })
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
                var blogpost = (await blogPostStore.SelectAsync(connection, 
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
                    r => r.BlogPostId == id && r.IsApproved.Value == true,
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
                })
            });
        }
    }
}
