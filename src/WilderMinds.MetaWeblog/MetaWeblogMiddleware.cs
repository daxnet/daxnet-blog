using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WilderMinds.MetaWeblog
{
    public class MetaWeblogMiddleware
    {
        private ILogger _logger;
        private readonly RequestDelegate _next;
        private MetaWeblogService _service;
        private string _urlEndpoint;

        public MetaWeblogMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, string urlEndpoint, MetaWeblogService service)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<MetaWeblogMiddleware>(); ;
            _urlEndpoint = urlEndpoint;
            _service = service;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == "POST" &&
              context.Request.Path.StartsWithSegments(_urlEndpoint) &&
              context.Request != null &&
              context.Request.ContentType.ToLower().Contains("text/xml"))
            {
                var rdr = new StreamReader(context.Request.Body);
                var xml = rdr.ReadToEnd();
                _logger.LogInformation($"Request XMLRPC: {xml}");
                var result = _service.Invoke(xml);
                _logger.LogInformation($"Result XMLRPC: {result}");
                context.Response.ContentType = "text/xml";
                context.Response.ContentLength = Encoding.UTF8.GetBytes(result).Length;
                await context.Response.WriteAsync(result, Encoding.UTF8);

                // This is only a workaround for the System.IOException issue
                // occurred in the Windows Live Writer client.
                await Task.Delay(500);
            }

            // Continue On
            await _next.Invoke(context);
        }
    }
}
