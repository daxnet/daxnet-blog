using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DaxnetBlog.Web
{
    /// <summary>
    /// Represents the HTTP client proxy.
    /// </summary>
    public sealed class ServiceProxy : HttpClient
    {
        private readonly IOptions<WebsiteSettings> config;

        public ServiceProxy(IOptions<WebsiteSettings> config)
            : base(new HttpClientHandler(), true)
        {
            this.config = config;
            var baseUrl = Environment.GetEnvironmentVariable("DAXNETBLOG_SVC_BASEURL");
            if (string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = config.Value.BaseUri;
            }
            this.BaseAddress = new Uri(baseUrl);
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
