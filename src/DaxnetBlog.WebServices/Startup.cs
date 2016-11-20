using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DaxnetBlog.Common.Storage;
using DaxnetBlog.Storage.SqlServer;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DaxnetBlog.WebServices.Middlewares;
using Serilog;
using DaxnetBlog.Common;

namespace DaxnetBlog.WebServices
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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<DefaultModule>();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Log.Logger = CreateLogger();

            loggerFactory.AddSerilog();

            app.UseMiddleware<CustomExceptionHandlingMiddleware>();
            app.UseMiddleware<CustomServiceResponseTimeMiddleware>();

            app.UseMvc();
        }

        private Serilog.ILogger CreateLogger()
        {
            var loggerConfig = new LoggerConfiguration();
            switch (EnvironmentVariables.SeqLoggerLevel.ToUpper())
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
