using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;

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

        /// <summary>
        /// Initializes a new instance of <see cref="VersionController"/> instance.
        /// </summary>
        /// <param name="hostingEnv"></param>
        public VersionController(IHostingEnvironment hostingEnv)
        {
            this.hostingEnvironment = hostingEnv;
        }

        /// <summary>
        /// Returns the version number of the current service deployment.
        /// </summary>
        /// <returns>The version number.</returns>
        [HttpGet]
        public string Get()
        {
            return System.IO.File.ReadAllText(Path.Combine(this.hostingEnvironment.WebRootPath, "version.txt"));
        }
    }
}
