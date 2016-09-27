using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DaxnetBlog.Common.Storage;
using DaxnetBlog.Domain.Model;

namespace DaxnetBlog.Domain.EntityStore
{
    public class AccountStore : EntityStore<Account, int>, IAccountStore
    {
        public AccountStore(IStoreMapping mapping, StorageDialectSettings dialectSettings) : base(mapping, dialectSettings)
        {
        }

        public Account GetAccountByUserName(string userName, IDbConnection connection) => 
            Select(connection, a => a.UserName == userName).FirstOrDefault();
    }
}
