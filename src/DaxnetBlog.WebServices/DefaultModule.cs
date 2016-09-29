using Autofac;
using DaxnetBlog.Common.Storage;
using DaxnetBlog.Domain.EntityStore;
using DaxnetBlog.Domain.Model;
using DaxnetBlog.Storage.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaxnetBlog.WebServices
{
    public class DefaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PluralTableNameStoreMapping>().As<IStoreMapping>();
            builder.Register(x => new SqlServerEntityStore<Account, int>(x.Resolve<IStoreMapping>(), x.Resolve<IStorage>().DialectSettings)).As<IEntityStore<Account, int>>();
            builder.RegisterType<SqlServerStorage>()
                .As<IStorage>()
                .WithParameter("connectionString", @"Server=localhost; Database=DaxnetBlogDB; Integrated Security=SSPI;");
        }
    }
}
