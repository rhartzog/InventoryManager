using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;

namespace DIMS.Data.DataAccess
{
    public interface IUnitOfWorkFactory
	{
		IUnitOfWork CreateUnitOfWork();
	}

	public class UnitOfWorkFactory : IUnitOfWorkFactory
	{
		private readonly string _connectionString;
        private readonly Lazy<ISessionFactory> _sessionFactory;

		public UnitOfWorkFactory(string connectionString)
		{
			_connectionString = connectionString;
		    _sessionFactory = new Lazy<ISessionFactory>(CreateSessionFactory);
		}

		private ISessionFactory CreateSessionFactory()
		{
			return Fluently.Configure()
					.Database(MsSqlConfiguration.MsSql2012.Driver<SqlClientDriver>().ConnectionString(_connectionString))
					.Mappings(m => m.FluentMappings.AddFromAssemblyOf<BoxMap>())
                    .ExposeConfiguration(BuildSchema) //Comment out if you don't want the schema automatically built
					.BuildSessionFactory();
		}

		public virtual IUnitOfWork CreateUnitOfWork()
		{
			return new UnitOfWork(_sessionFactory.Value);
		}

	    private static void BuildSchema(Configuration config)
	    {
	        new SchemaExport(config)
                .Create(false, true);
	    }
	}
}