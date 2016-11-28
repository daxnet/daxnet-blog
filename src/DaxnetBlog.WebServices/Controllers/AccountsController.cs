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

using DaxnetBlog.Common;
using DaxnetBlog.Common.Storage;
using DaxnetBlog.Domain.Model;
using DaxnetBlog.WebServices.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace DaxnetBlog.WebServices.Controllers
{
    /// <summary>
    /// Represents the API controller that provides account accessibility features.
    /// </summary>
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private readonly IStorage storage;
        private readonly IEntityStore<Account, int> accountStore;

        /// <summary>
        /// Initializes a new instance of <c>AccountsController</c> class.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="accountStore"></param>
        public AccountsController(IStorage storage, IEntityStore<Account, int> accountStore)
        {
            this.storage = storage;
            this.accountStore = accountStore;
        }

        /// <summary>
        /// Creates an account by using the given model.
        /// </summary>
        /// <param name="accountObject"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAccount([FromBody] dynamic accountObject)
        {
            var userName = (string)accountObject.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, Reason.ArgumentNull, "参数userName不能为空。");
            }
            var password = (string)accountObject.Password;
            if (string.IsNullOrEmpty(password))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, Reason.ArgumentNull, "参数password不能为空。");
            }

            var passwordHash = Crypto.ComputeHash(password, userName);

            var email = (string)accountObject.Email;
            if (string.IsNullOrEmpty(email))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, Reason.ArgumentNull, "参数email不能为空。");
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
                    throw new ServiceException(HttpStatusCode.Conflict, Reason.AlreadyExists, $"用户名 {userName} 已经存在。");
                }

                var userWithEmail = (await accountStore.SelectAsync(connection,
                    x => x.EmailAddress == email,
                    transaction: transaction,
                    cancellationToken: cancellationToken)).FirstOrDefault();
                if (userWithEmail != null)
                {
                    throw new ServiceException(HttpStatusCode.Conflict, Reason.AlreadyExists, $"电子邮件地址 {email} 已经存在。");
                }

                var verificationCode = Utils.GetUniqueStringValue(16);

                var rowsAffected = await accountStore.InsertAsync(
                   new Account
                   {
                       UserName = userName,
                       PasswordHash = passwordHash,
                       EmailAddress = email,
                       NickName = nickName,
                       DateRegistered = DateTime.UtcNow,
                       EmailVerifyCode = verificationCode,
                       IsLocked = true,
                       IsAdmin = false
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
                        throw new ServiceException(Reason.CreateFailed, "未能成功创建用户帐号。");
                    }
                    return insertedAccount.Id;
                }
                return 0;
            });

            if (result > 0)
            {
                var uri = Url.Action("GetById", new { id = result });
                return Created(uri, result);
            }
            else
            {
                throw new ServiceException(Reason.CreateFailed, "未能成功创建用户帐号。");
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var account = (await storage.ExecuteAsync(async (connection, cancellationToken) =>
                await accountStore.SelectAsync(connection, x => x.Id == id, cancellationToken: cancellationToken))).FirstOrDefault();

            if (account == null)
            {
                throw new ServiceException(HttpStatusCode.NotFound, Reason.EntityNotFound, $"未能找到用户ID为 {id} 的用户。");
            }

            return Ok(new {
                account.Id,
                account.UserName,
                account.NickName,
                account.EmailAddress,
                account.DateRegistered,
                account.DateLastLogin,
                account.EmailVerifyCode,
                account.EmailVerifiedDate,
                account.IsLocked,
                account.IsAdmin
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
                throw new ServiceException(HttpStatusCode.NotFound, Reason.EntityNotFound, $"未能找到用户名为 {name} 的用户。");
            }

            return Ok(new
            {
                account.Id,
                account.UserName,
                account.NickName,
                account.EmailAddress,
                account.DateRegistered,
                account.DateLastLogin,
                account.EmailVerifyCode,
                account.EmailVerifiedDate,
                account.IsLocked,
                account.IsAdmin
            });
        }

        [HttpGet]
        [Route("paginate/{pageSize}/{pageNumber}")]
        public async Task<IActionResult> GetByPaging(int pageSize, int pageNumber)
        {
            var pagedModel = await this.storage.ExecuteAsync(async (connection, cancellationToken) =>
                await accountStore.SelectAsync(pageNumber, pageSize, connection, new Sort<Account, int> { { x => x.DateRegistered, SortOrder.Descending } }, cancellationToken: cancellationToken)
            );

            return Ok(new
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalRecords = pagedModel.TotalRecords,
                TotalPages = pagedModel.TotalPages,
                Count = pagedModel.Count,
                Data = pagedModel.Select(x => new
                {
                    x.Id,
                    x.UserName,
                    x.NickName,
                    x.DateRegistered,
                    x.IsAdmin,
                    x.IsLocked,
                    x.EmailAddress,
                    x.EmailVerifyCode,
                    x.EmailVerifiedDate
                })
            });
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await this.storage.ExecuteAsync(async (connection, cancellationToken) =>
                await accountStore.SelectAsync(connection, sorting: new Sort<Account, int> { { acct => acct.DateRegistered, SortOrder.Descending } }, cancellationToken: cancellationToken)));
        }

        [HttpGet]
        [Route("authenticate/passwordhash/{id}")]
        public async Task<IActionResult> GetPasswordHash(int id)
        {
            var account = (await storage.ExecuteAsync(async (connection, cancellationToken) =>
                await accountStore.SelectAsync(connection, x => x.Id == id, cancellationToken: cancellationToken))).FirstOrDefault();

            if (account == null)
            {
                throw new ServiceException(HttpStatusCode.NotFound, Reason.EntityNotFound, $"No account was found with the id of {id}.");
            }

            return Ok(account.PasswordHash);
        }

        /// <summary>
        /// Authenticates the user with the specified identifier, by using the provided password.
        /// </summary>
        /// <param name="id">The identifier of the user to be authenticated.</param>
        /// <param name="passwordModel">The password model.</param>
        /// <returns></returns>
        /// <exception cref="ServiceException">
        /// The password argument cannot be null.
        /// or
        /// No account was found with the id of {id}.
        /// </exception>
        [HttpPost]
        [Route("authenticate/{id}")]
        public async Task<IActionResult> Authenticate(int id, [FromBody] dynamic passwordModel)
        {
            var password = (string)passwordModel.Password;
            if (string.IsNullOrEmpty(password))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, Reason.ArgumentNull, "The password argument cannot be null.");
            }

            var account = (await storage.ExecuteAsync(async (connection, cancellationToken) =>
                await accountStore.SelectAsync(connection, x => x.Id == id, cancellationToken: cancellationToken))).FirstOrDefault();

            if (account == null)
            {
                throw new ServiceException(HttpStatusCode.NotFound, Reason.EntityNotFound, $"No account was found with the id of {id}.");
            }

            return Ok(account.ValidatePassword(password));
        }

        [HttpPost]
        [Route("authenticate/username/{userName}")]
        public async Task<IActionResult> AuthenticateByUserName(string userName, [FromBody] dynamic passwordModel)
        {
            var password = (string)passwordModel.Password;
            if (string.IsNullOrEmpty(password))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, Reason.ArgumentNull, "The password argument cannot be null.");
            }

            var account = (await storage.ExecuteAsync(async (connection, cancellationToken) =>
                await accountStore.SelectAsync(connection, x => x.UserName == userName, cancellationToken: cancellationToken))).FirstOrDefault();

            if (account == null)
            {
                throw new ServiceException(HttpStatusCode.NotFound, Reason.EntityNotFound, $"No account was found with the userName of {userName}.");
            }

            return Ok(account.ValidatePassword(password));
        }

        /// <summary>
        /// Registers the login information for the account authentication.
        /// </summary>
        /// <param name="model">The model which contains the account name.</param>
        /// <returns>
        /// HTTP 200: Update succeeded.
        /// HTTP 400: The name of the account has not been specified in the request body.
        /// HTTP 404: The account with the specified name does not exist.
        /// HTTP 500: Update failed.
        /// </returns>
        [HttpPost]
        [Route("authenticate/login")]
        public async Task<IActionResult> RegisterForLogin([FromBody] dynamic model)
        {
            var userName = (string)model.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, Reason.ArgumentNull, "请求数据中未指定用户ID值。");
            }

            var rowsAffected = await this.storage.ExecuteAsync(async (connection, transaction, cancellationToken) =>
            {
                var account = (await this.accountStore.SelectAsync(connection, 
                    acct => acct.UserName == userName, 
                    transaction: transaction, 
                    cancellationToken: cancellationToken)).FirstOrDefault();

                if (account == null)
                {
                    throw new ServiceException(HttpStatusCode.NotFound, Reason.EntityNotFound, $"未能找到帐号名称为{userName}的用户帐号。");
                }

                account.DateLastLogin = DateTime.UtcNow;
                return await this.accountStore.UpdateAsync(account,
                    connection,
                    acct => acct.UserName == userName,
                    new Expression<Func<Account, object>>[] { acct => acct.DateLastLogin },
                    transaction, cancellationToken);
            });

            if (rowsAffected > 0)
            {
                return Ok();
            }
            throw new ServiceException(Reason.UpdateFailed, $"未能为用户帐号{userName}成功更新最近登录日期（DateLastLogin）。");
        }

        [HttpGet]
        [Route("verification/code/{userName}")]
        public async Task<IActionResult> GetEmailVerificationCode(string userName)
        {
            var result = await storage.ExecuteAsync(async (connection, cancellationToken) =>
            {
                var account = (await accountStore.SelectAsync(connection, u => u.UserName == userName)).FirstOrDefault();
                if (account == null)
                {
                    throw new ServiceException(HttpStatusCode.NotFound, Reason.EntityNotFound, $"用户 {userName} 不存在。");
                }
                return account.EmailVerifyCode;
            });
            return Ok(result);
        }

        [HttpPost]
        [Route("verification/verify")]
        public async Task<IActionResult> Verify([FromBody] dynamic model)
        {
            var userName = (string)model.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, Reason.ArgumentNull, "UserId cannot be null.");
            }

            var code = (string)model.Code;
            if (string.IsNullOrEmpty(code))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, Reason.ArgumentNull, "Code cannot be null.");
            }

            var result = await storage.ExecuteAsync(async (connection, transaction, cancellationToken) =>
            {
                var account = (await accountStore.SelectAsync(connection, 
                    u => u.UserName == userName && u.EmailVerifyCode == code, 
                    transaction: transaction, 
                    cancellationToken: cancellationToken)).FirstOrDefault();
                if (account != null)
                {
                    account.IsLocked = false;
                    account.EmailVerifiedDate = DateTime.UtcNow;

                    var rowsAffected = await accountStore.UpdateAsync(account,
                        connection,
                        x => x.UserName == userName,
                        new Expression<Func<Account, object>>[] { x => x.IsLocked, x => x.EmailVerifiedDate },
                        transaction,
                        cancellationToken);

                    return rowsAffected > 0;
                }
                return false;
            });

            return Ok(result);
        }


        [HttpPost]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] dynamic model)
        {
            var nickName = (string)model.NickName;
            var emailAddress = (string)model.EmailAddress;

            var result = await storage.ExecuteAsync(async (connection, transaction, cancellationToken) =>
            {
                var account = (await accountStore.SelectAsync(connection,
                    x => x.Id == id,
                    transaction: transaction,
                    cancellationToken: cancellationToken)).FirstOrDefault();
                if (account == null)
                {
                    throw new ServiceException(HttpStatusCode.NotFound, Reason.EntityNotFound, "Account does not exist.");
                }

                var updateFields = new List<Expression<Func<Account, object>>>();
                if (!string.IsNullOrEmpty(nickName))
                {
                    account.NickName = nickName;
                    updateFields.Add(x => x.NickName);
                }

                if (!string.IsNullOrEmpty(emailAddress))
                {
                    account.EmailAddress = emailAddress;
                    updateFields.Add(x => x.EmailAddress);
                }

                return updateFields.Count > 0 ?
                    await accountStore.UpdateAsync(account,
                        connection,
                        x => x.Id == id,
                        updateFields,
                        transaction,
                        cancellationToken) : 0;
            });

            return Ok(result);
        }

        [HttpPost]
        [Route("password/change/{id}")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] dynamic model)
        {
            var oldPassword = (string)model.OldPassword;
            var newPassword = (string)model.NewPassword;
            if (string.IsNullOrEmpty(oldPassword))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, Reason.ArgumentNull, "修改密码时未提供旧密码。");
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ServiceException(HttpStatusCode.BadRequest, Reason.ArgumentNull, "修改密码时未提供新密码。");
            }

            var affectedRows = await storage.ExecuteAsync(async (connection, transaction, cancellationToken) =>
            {
                var account = (await accountStore.SelectAsync(connection, acct => acct.Id == id, transaction: transaction, cancellationToken: cancellationToken)).FirstOrDefault();
                if (account == null)
                {
                    throw new ServiceException(HttpStatusCode.NotFound, Reason.EntityNotFound, $"未能找到ID为{id}的用户帐号。");
                }

                if (account.ValidatePassword(oldPassword))
                {
                    var passwordHash = Crypto.ComputeHash(newPassword, account.UserName);
                    account.PasswordHash = passwordHash;

                    return await accountStore.UpdateAsync(account, connection, acct => acct.Id == id,
                        new Expression<Func<Account, object>>[] { acct => acct.PasswordHash }, transaction, cancellationToken);
                }
                else
                {
                    throw new ServiceException(HttpStatusCode.MethodNotAllowed, Reason.InvalidArgument, "指定的旧密码不正确。");
                }
            });

            if (affectedRows > 0)
                return Ok(affectedRows);
            throw new ServiceException(Reason.UpdateFailed, "密码更新失败。");
        }
    }
}
