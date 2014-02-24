using System;
using System.Linq;

namespace DIMS.Data.DataAccess
{
    public interface IUnitOfWork : IDisposable
	{
		bool IsInActiveTransaction { get; }
		bool IsStarted { get; }
		void Commit();
		void Create(object entity);
		void Update(object entity);
		void Delete(object entity);
		void Flush();

		IQueryable<T> Query<T>();
		IUnitOfWork Start();
	}
}
