using DaxnetBlog.Common;
using DaxnetBlog.Common.Storage;
using DaxnetBlog.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace DaxnetBlog.WebServices.Controllers
{
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private readonly IStorage storage;
        private readonly IEntityStore<Account, int> accountStore;

        public AccountsController(IStorage storage, IEntityStore<Account, int> accountStore)
        {
            this.storage = storage;
            this.accountStore = accountStore;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAccount([FromBody] dynamic accountObject)
        {
            var userName = (string)accountObject.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, "userName cannot be null.");
            }
            var password = (string)accountObject.Password;
            if (string.IsNullOrEmpty(password))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, "password cannot be null.");
            }

            var passwordHash = Crypto.ComputeHash(password, userName);

            var email = (string)accountObject.Email;
            if (string.IsNullOrEmpty(email))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, "email cannot be null.");
            }
            var nickName = (string)accountObject.NickName;
            if (string.IsNullOrEmpty(nickName))
            {
                nickName = userName;
            }

            var result = await storage.ExecuteAsync(async(connection, transaction, cancellationToken) => 
            {
                var userWithName = (await accountStore.SelectAsync(connection, 
                    x => x.UserName == userName, 
                    transaction: transaction, 
                    cancellationToken: cancellationToken)).FirstOrDefault();
                if (userWithName != null)
                {
                    throw new ServiceException(HttpStatusCode.Conflict, $"用户名 {userName} 已经存在。", false);
                }

                var userWithEmail = (await accountStore.SelectAsync(connection,
                    x => x.EmailAddress == email,
                    transaction: transaction,
                    cancellationToken: cancellationToken)).FirstOrDefault();
                if (userWithEmail != null)
                {
                    throw new ServiceException(HttpStatusCode.Conflict, $"电子邮件地址 {email} 已经存在。", false);
                }

                var rowsAffected = await accountStore.InsertAsync(
                   new Account
                   {
                       UserName = userName,
                       PasswordHash = passwordHash,
                       EmailAddress = email,
                       NickName = nickName,
                       DateRegistered = DateTime.UtcNow
                   },
                   connection,
                   new Expression<Func<Account, object>>[] { a => a.Id },
                   transaction, cancellationToken);

                if (rowsAffected > 0)
                {
                    var insertedAccount = (await accountStore.SelectAsync(connection, 
                        x => x.UserName == userName,
                        new Sort<Account, int> { { x=>x.DateRegistered, SortOrder.Descending } }, // Gets the last record inserted, if any duplicates
                        transaction: transaction, 
                        cancellationToken: cancellationToken)).FirstOrDefault();

                    if (insertedAccount == null)
                    {
                        throw new ServiceException("No account was created in the current transaction.");
                    }
                    return insertedAccount.Id;
                }
                return 0;
            });

            var uri = Url.Action("GetById", new { id = result });
            return Created(uri, result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var account = (await storage.ExecuteAsync(async (connection, cancellationToken) =>
                await accountStore.SelectAsync(connection, x => x.Id == id, cancellationToken: cancellationToken))).FirstOrDefault();

            if (account == null)
            {
                throw new ServiceException(HttpStatusCode.NotFound, $"No account was found with the id of {id}.");
            }

            return Ok(new {
                account.Id,
                account.UserName,
                account.NickName,
                account.EmailAddress,
                account.DateRegistered,
                account.DateLastLogin
            });
        }

        [HttpGet]
        [Route("name/{name}")]
        public async Task<IActionResult> GetByUserName(string name)
        {
            var account = (await storage.ExecuteAsync(async (connection, cancellationToken) =>
                await accountStore.SelectAsync(connection, x => x.UserName == name, cancellationToken: cancellationToken))).FirstOrDefault();

            if (account == null)
            {
                throw new ServiceException(HttpStatusCode.NotFound, $"No account was found with the userName of {name}.");
            }

            return Ok(new
            {
                account.Id,
                account.UserName,
                account.NickName,
                account.EmailAddress,
                account.DateRegistered,
                account.DateLastLogin
            });
        }

        [HttpGet]
        [Route("pwd/{id}")]
        public async Task<IActionResult> GetPasswordHash(int id)
        {
            var account = (await storage.ExecuteAsync(async (connection, cancellationToken) =>
                await accountStore.SelectAsync(connection, x => x.Id == id, cancellationToken: cancellationToken))).FirstOrDefault();

            if (account == null)
            {
                throw new ServiceException(HttpStatusCode.NotFound, $"No account was found with the id of {id}.");
            }

            return Ok(account.PasswordHash);
        }

        [HttpPost]
        [Route("authenticate/{id}")]
        public async Task<IActionResult> Authenticate(int id, [FromBody] dynamic passwordModel)
        {
            var password = (string)passwordModel.Password;
            if (string.IsNullOrEmpty(password))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, "The password argument cannot be null.");
            }

            var account = (await storage.ExecuteAsync(async (connection, cancellationToken) =>
                await accountStore.SelectAsync(connection, x => x.Id == id, cancellationToken: cancellationToken))).FirstOrDefault();

            if (account == null)
            {
                throw new ServiceException(HttpStatusCode.NotFound, $"No account was found with the id of {id}.");
            }

            return Ok(account.ValidatePassword(password));
        }
    }
}
