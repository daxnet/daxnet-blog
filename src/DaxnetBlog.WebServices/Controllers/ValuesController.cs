using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DaxnetBlog.Common.Storage;
using DaxnetBlog.Domain.EntityStore;
using DaxnetBlog.Domain.Model;

namespace DaxnetBlog.WebServices.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IStorage storage;
        private readonly IAccountStore accountStore;

        public ValuesController(IStorage storage, IAccountStore accountStore)
        {
            this.storage = storage;
            this.accountStore = accountStore;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            IEnumerable<Account> accounts = null;
            storage.Execute(connection =>
            {
                accounts = accountStore.Select(connection, x => x.PasswordHash == "123", new Sort<Account, int> { { x => x.UserName, SortOrder.Ascending } });
            });

            return accounts.Select(x => x.UserName);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
