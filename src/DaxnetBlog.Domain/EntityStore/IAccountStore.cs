using DaxnetBlog.Common.Storage;
using DaxnetBlog.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.Domain.EntityStore
{
    public interface IAccountStore : IEntityStore<Account, int>
    {
        Account GetAccountByUserName(string userName, IDbConnection connection);
    }
}
