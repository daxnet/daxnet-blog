// ===========================================================================================================
//      _                                 _              _       _                 
//     | |                               | |            | |     | |                
//   __| |   __ _  __  __  _ __     ___  | |_   ______  | |__   | |   ___     __ _ 
//  / _` |  / _` | \ \/ / | '_ \   / _ \ | __| |______| | '_ \  | |  / _ \   / _` |
// | (_| | | (_| |  >  <  | | | | |  __/ | |_           | |_) | | | | (_) | | (_| |
//  \__,_|  \__,_| /_/\_\ |_| |_|  \___|  \__|          |_.__/  |_|  \___/   \__, |
//                                                                            __/ |
//                                                                           |___/ 
//
// 
// Daxnet Personal Blog
// Copyright © 2016 by daxnet (Sunny Chen)
//
// https://github.com/daxnet/daxnet-blog
//
// MIT License
// 
// Copyright(c) 2016 Sunny Chen
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ===========================================================================================================

using DaxnetBlog.Common.IntegrationServices;
using DaxnetBlog.WebServices.Caching;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

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
