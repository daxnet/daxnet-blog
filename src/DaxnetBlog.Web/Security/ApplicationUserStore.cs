using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;

namespace DaxnetBlog.Web.Security
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Identity.IUserStore{DaxnetBlog.Web.Security.User}" />
    public class ApplicationUserStore : IUserStore<User>, IUserPasswordStore<User>
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationUserStore"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        public ApplicationUserStore(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            // TODO: Implement create feature later.
            return Task.FromResult(new IdentityResult());
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            // TODO: Implement create feature later.
            return Task.FromResult(new IdentityResult());
        }

        public void Dispose()
        {
            this.httpClient.Dispose();
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await (await this.httpClient.GetAsync($"accounts/{userId}", cancellationToken))
                .Content
                .ReadAsAsync<User>();
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = await (await this.httpClient.GetAsync($"accounts/name/{normalizedUserName}", cancellationToken))
                .Content
                .ReadAsAsync<User>();
            return user;
        }

        public async Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return await Task.FromResult(user.NickName);
        }

        public async Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            var pwdHash = await (await this.httpClient.GetAsync($"accounts/pwd/{user.Id}", cancellationToken)).Content.ReadAsStringAsync();
            return pwdHash;
        }

        public async Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return await Task.FromResult(user.Id.ToString());
        }

        public async Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return await Task.FromResult(user.UserName);
        }

        public async Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return await Task.FromResult(true);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NickName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
