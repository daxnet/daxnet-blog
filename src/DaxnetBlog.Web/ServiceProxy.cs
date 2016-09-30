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
            this.BaseAddress = new Uri(config.Value.BaseUri);
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
