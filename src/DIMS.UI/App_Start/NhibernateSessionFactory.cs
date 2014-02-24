using DIMS.Data;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace DIMS.UI
{
    public class NhibernateSessionFactory
    {
        public ISessionFactory GetSessionFactory()
        {
            var fluentConfiguration = Fluently.Configure()
                .Database(
                    MsSqlConfiguration.MsSql2008.ConnectionString(c => c.FromConnectionStringWithKey("DIMS")))
                .Mappings(m => m.FluentMappings
                    .AddFromAssemblyOf<BoxMap>())
                .ExposeConfiguration(BuidSchema)
                .BuildSessionFactory();

            return fluentConfiguration;
        }

        //WARNING THIS WILL TRY TO DROP ALL YOUR TABLES EVERYTIME YOU LAUNCH YOUR SITE. DISABLE AFTER CREATING DATABASE.
        private static void BuidSchema(NHibernate.Cfg.Configuration config)
        {
            new NHibernate.Tool.hbm2ddl.SchemaUpdate(config).Execute(false, true);
        }
    }
}