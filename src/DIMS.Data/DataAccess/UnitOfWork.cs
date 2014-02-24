using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;

namespace DIMS.Data.DataAccess
{
    public sealed class UnitOfWork : IUnitOfWork
	{
		private readonly ISessionFactory _sessionFactory;

		private ISession _session;

		public UnitOfWork(ISessionFactory sessionFactory)
		{
			_sessionFactory = sessionFactory;
		}

		public bool IsInActiveTransaction
		{
			get
			{
				return _session.Transaction.IsActive;
			}
		}

		public bool IsStarted
		{
			get
			{
				return _session != null;
			}
		}

		public Transaction BeginTransaction()
		{
			return new Transaction(_session.BeginTransaction());
		}

		public void Commit()
		{
			var tx = BeginTransaction();

			try
			{
				tx.Commit();
			}
			catch
			{
				tx.Rollback();
				throw;
			}
			finally
			{
				tx.Dispose();
			}
		}

		public void Create(object entity)
		{
			_session.Save(entity);
		}

		public void Delete(object entity)
		{
			_session.Delete(entity);
		}

		public void Dispose()
		{
			if (_session != null)
			{
				_session.Flush();
				_session.Dispose();
			}

			_session = null;
		}

		public void Flush()
		{
			_session.Flush();
		}

		/// <summary>
		/// Provides LINQ.
		/// </summary>
		public IQueryable<T> Query<T>()
		{
			if (_session == null)
			{
				throw new Exception("No nhibernate session to perform query.");
			}

			return _session.Query<T>();
		}

		public IUnitOfWork Start()
		{
			_session = CreateSession();

			_session.FlushMode = FlushMode.Commit;

			return this;
		}

		public void Update(object entity)
		{
			_session.Update(entity);
		}

		private ISession CreateSession()
		{
			return _sessionFactory.OpenSession();
		}
	}
}
