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
    [Route("api/[controller]")]
    public class VersionController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public VersionController(IHostingEnvironment hostingEnv)
        {
            this.hostingEnvironment = hostingEnv;
        }

        [HttpGet]
        public string Get()
        {
            return System.IO.File.ReadAllText(Path.Combine(this.hostingEnvironment.WebRootPath, "version.txt"));
        }
    }
}
