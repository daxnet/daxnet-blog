using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading;

namespace DaxnetBlog.Web.Security
{
    public class ApplicationUserManager : UserManager<User>
    {
        private readonly HttpClient httpClient;

        public ApplicationUserManager(HttpClient httpClient, 
            IUserStore<User> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<User> passwordHasher, 
            IEnumerable<IUserValidator<User>> userValidators, 
            IEnumerable<IPasswordValidator<User>> passwordValidators, 
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ILogger<UserManager<User>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            this.httpClient = httpClient;
        }

        public override async Task<bool> CheckPasswordAsync(User user, string password)
        {
            string trueFalseResult;
            if (user.Id > 0)
            {
                trueFalseResult = await (await this.httpClient.PostAsJsonAsync($"accounts/authenticate/{user.Id}", new { Password = password })).Content.ReadAsStringAsync();
            }
            else if (!string.IsNullOrEmpty(user.UserName))
            {
                trueFalseResult = await (await this.httpClient.PostAsJsonAsync($"accounts/authenticate/username/{user.UserName}", new { Password = password })).Content.ReadAsStringAsync();
            }
            else
            {
                throw new ArgumentException("Either Id or UserName is not specified.");
            }

            return bool.Parse(trueFalseResult);
        }

        public override async Task<IdentityResult> CreateAsync(User user, string password)
        {
            var result = await this.httpClient.PostAsJsonAsync($"accounts/create", new
            {
                UserName = user.UserName,
                Password = password,
                Email = user.EmailAddress,
                NickName = user.NickName
            });

            if (result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return IdentityResult.Success;
            }

            var message = await result.Content.ReadAsStringAsync();
            return IdentityResult.Failed(new[] { new IdentityError
            {
                Code = result.StatusCode.ToString(),
                Description = message
            } });
        }

        public override async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await (await this.httpClient.GetAsync($"accounts/verification/code/{user.UserName}")).Content.ReadAsStringAsync();
        }

        public override async Task<bool> GetLockoutEnabledAsync(User user)
        {
            return await Task.FromResult(true);
        }

        public override async Task<bool> IsLockedOutAsync(User user)
        {
            var account = await Store.FindByNameAsync(user.UserName, default(CancellationToken));
            return !account.IsLocked.HasValue || account.IsLocked.Value;
        }

        public override async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            var result = await httpClient.PostAsJsonAsync("accounts/verification/verify", new { UserName = user.UserName, Code = token });
            try
            {
                result.EnsureSuccessStatusCode();
                var verified = Convert.ToBoolean(await result.Content.ReadAsStringAsync());
                return verified ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "请检查所提供的用户信息是否正确。" });
            }
            catch(Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"错误信息：{ex.Message}。" });
            }
        }

        public override async Task<IdentityResult> UpdateAsync(User user)
        {
            var result = await httpClient.PostAsJsonAsync($"accounts/update/{user.Id}", new { NickName = user.NickName, EmailAddress = user.EmailAddress });
            try
            {
                result.EnsureSuccessStatusCode();
                return IdentityResult.Success;
            }
            catch(Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"错误信息：{ex.Message}。" });
            }
        }

        public override Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return base.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public override bool SupportsUserLockout
        {
            get
            {
                return true;
            }
        }
    }
}
