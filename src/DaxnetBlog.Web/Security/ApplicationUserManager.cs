using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;

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
            var trueFalseResult = await (await this.httpClient.PostAsJsonAsync($"accounts/authenticate/{user.Id}", new { Password = password })).Content.ReadAsStringAsync();
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
    }
}
