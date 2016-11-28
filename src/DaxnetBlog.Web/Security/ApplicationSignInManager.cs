using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace DaxnetBlog.Web.Security
{
    /// <summary>
    /// Represents the custom sign in manager.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Identity.SignInManager{DaxnetBlog.Web.Security.User}" />
    public sealed class ApplicationSignInManager : SignInManager<User>
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSignInManager"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="contextAccessor">The context accessor.</param>
        /// <param name="claimsFactory">The claims factory.</param>
        /// <param name="optionsAccessor">The options accessor.</param>
        /// <param name="logger">The logger.</param>
        public ApplicationSignInManager(HttpClient httpClient,
            UserManager<User> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<User> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<User>> logger) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Attempts to sign in the specified <paramref name="userName" /> and <paramref name="password" /> combination
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="userName">The user name to sign in.</param>
        /// <param name="password">The password to attempt to sign in with.</param>
        /// <param name="isPersistent">Flag indicating whether the sign-in cookie should persist after the browser is closed.</param>
        /// <param name="lockoutOnFailure">Flag indicating if the user account should be locked if the sign in fails.</param>
        /// <returns>
        /// The task object representing the asynchronous operation containing the <see name="SignInResult" />
        /// for the sign-in attempt.
        /// </returns>
        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var signInResult = await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
            if (signInResult.Succeeded)
            {
                var updateResult = await this.httpClient.PostAsJsonAsync("accounts/authenticate/login", new { UserName = userName });
                updateResult.EnsureSuccessStatusCode();
            }
            return signInResult;
        }
    }
}
