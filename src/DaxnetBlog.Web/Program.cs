using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography.X509Certificates;
using DaxnetBlog.Common;

namespace DaxnetBlog.Web
{
    public class Program
    {
        const string SslCertificateFileName = @"daxnetme.pfx";

        public static void Main(string[] args)
        {
            var sslCertificatePassword = EnvironmentVariables.SslCertificatePassword;
            IWebHostBuilder builder = new WebHostBuilder();

            if (string.IsNullOrEmpty(sslCertificatePassword))
            {
                builder = builder.UseKestrel().UseUrls("http://+:5000/");
            }
            else
            {
                builder = builder.UseKestrel(options =>
                    options.UseHttps(new X509Certificate2(SslCertificateFileName, sslCertificatePassword)))
                .UseUrls("http://+:5000/", "https://+:5001/");
            }
            
            var host = builder
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
