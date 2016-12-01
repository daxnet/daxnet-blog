using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DaxnetBlog.Web.Security;
using System.Net.Http;
using WilderMinds.MetaWeblog;
using DaxnetBlog.Common.IntegrationServices;
using DaxnetBlog.AzureServices;
using DaxnetBlog.Common;
using DaxnetBlog.Web.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace DaxnetBlog.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddMetaWeblog<MetaWeblogService>();

            services.AddTransient<HttpClient, ServiceProxy>();

            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IMediaObjectStorageService>(new AzureBlobStorageService(EnvironmentVariables.WebAzureStorageBaseUrl, 
                EnvironmentVariables.WebAzureStorageAccount, EnvironmentVariables.WebAzureStorageKey));

            // Build the configuration from configuration file.
            services.AddOptions();
            services.Configure<WebsiteSettings>(Configuration);
            
            services
                .AddIdentity<User, Role>()
                .AddUserStore<ApplicationUserStore>()
                .AddRoleStore<ApplicationRoleStore>()
                .AddUserManager<ApplicationUserManager>()
                .AddSignInManager<ApplicationSignInManager>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administration", policy => policy.AddRequirements(new PermissionKeyRequirement("Administration")));
            });

            services.AddTransient<IAuthorizationHandler, PermissionKeyAuthorizationHandler>();

            services.AddRouting(options => options.LowercaseUrls = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMiddleware<ApiAuthenticationMiddleware>();

            Log.Logger = CreateLogger();

            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseMiddleware<EnsureHttpsMiddleware>();
            }

            app.UseMetaWeblog("/api/metaweblog");

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private Serilog.ILogger CreateLogger()
        {
            var loggerConfig = new LoggerConfiguration();
            switch(EnvironmentVariables.SeqLoggerLevel.ToUpper())
            {
                case "INFORMATION":
                    loggerConfig = loggerConfig.MinimumLevel.Information();
                    break;
                case "WARNING":
                    loggerConfig = loggerConfig.MinimumLevel.Warning();
                    break;
                case "ERROR":
                    loggerConfig = loggerConfig.MinimumLevel.Error();
                    break;
            }
            return loggerConfig.Enrich.FromLogContext()
                .WriteTo.Seq(EnvironmentVariables.SeqLoggerUrl)
                .CreateLogger();
        }
    }
}
