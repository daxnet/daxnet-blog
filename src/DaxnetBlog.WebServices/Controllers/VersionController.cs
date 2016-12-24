using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using DaxnetBlog.Common.IntegrationServices;
using DaxnetBlog.WebServices.Caching;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace DaxnetBlog.WebServices.Controllers
{
    /// <summary>
    /// Represents the controller which will return the version number of the current deployment.
    /// </summary>
    [Route("api/[controller]")]
    public class VersionController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ICachingService cachingService;

        /// <summary>
        /// Initializes a new instance of <see cref="VersionController"/> instance.
        /// </summary>
        /// <param name="hostingEnv"></param>
        public VersionController(IHostingEnvironment hostingEnv,
            ICachingService cachingService)
        {
            this.hostingEnvironment = hostingEnv;
            this.cachingService = cachingService;
        }

        /// <summary>
        /// Returns the version number of the current service deployment.
        /// </summary>
        /// <returns>The version number.</returns>
        [HttpGet]
        public string Get()
        {
            var cachingKey = new CachingKey(CachingKeys.VERSION_NUMBER_KEY);
            var version = cachingService.Get(cachingKey);
            if (version != null)
            {
                return version.ToString();
            }

            var result = System.IO.File.ReadAllText(Path.Combine(this.hostingEnvironment.WebRootPath, "version.txt"));
            cachingService.Put(cachingKey, result);

            return result;
        }
    }
}
