using Autofac;
using DaxnetBlog.Common;
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
        const string DefaultConnectionString = @"Server=localhost; Database=DaxnetBlogDB; Integrated Security=SSPI;";
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be
        /// registered.</param>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        protected override void Load(ContainerBuilder builder)
        {
            var connectionString = Environment.GetEnvironmentVariable("DAXNETBLOG_SQL_STR");
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = DefaultConnectionString;
            }
            else
            {
                try
                {
                    var c = Crypto.Create(CryptoTypes.EncTypeTripleDes);
                    connectionString = c.Decrypt(connectionString, Crypto.GlobalKey);
                }
                catch
                {
                    connectionString = DefaultConnectionString;
                }
            }

            // Registers the store mapping instance.
            builder.RegisterType<PluralTableNameStoreMapping>()
                .As<IStoreMapping>();

            // Registers the entity store for Account entity.
            builder.Register(x => 
                    new SqlServerEntityStore<Account, int>(x.Resolve<IStoreMapping>(), x.Resolve<IStorage>().DialectSettings))
                .As<IEntityStore<Account, int>>();

            // Registers the entity store for BlogPost entity.
            builder.Register(x => 
                    new SqlServerEntityStore<BlogPost, int>(x.Resolve<IStoreMapping>(), x.Resolve<IStorage>().DialectSettings))
                .As<IEntityStore<BlogPost, int>>();

            // Registers the entity store for Reply entity.
            builder.Register(x =>
                    new SqlServerEntityStore<Reply, int>(x.Resolve<IStoreMapping>(), x.Resolve<IStorage>().DialectSettings))
                .As<IEntityStore<Reply, int>>();

            // Registers the SQL Server storage.
            builder.RegisterType<SqlServerStorage>()
                .As<IStorage>()
                .WithParameter("connectionString", connectionString);
        }
    }
}
