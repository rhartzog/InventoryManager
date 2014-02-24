using System;
using System.Linq;

namespace DIMS.Data.DataAccess
{
    public class Repository<TEntity> : IRepository<TEntity>
	{
		private readonly IUnitOfWork _unitOfWork;

		public Repository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork.Start();
		}

		public Type EntityType
		{
			get
			{
				return typeof(TEntity);
			}
		}

		public virtual bool Create(TEntity entity)
		{
			_unitOfWork.Create(entity);

			return true;
		}

		public virtual bool Delete(TEntity entity)
		{
			_unitOfWork.Delete(entity);

			return true;
		}

		public virtual bool Update(TEntity entity)
		{
			_unitOfWork.Update(entity);

			return true;
		}

		public IQueryable<TEntity> AsQueryable()
		{
			return _unitOfWork.Query<TEntity>();
		}

		public void SaveChanges()
		{
			if (_unitOfWork.IsInActiveTransaction)
			{
				_unitOfWork.Commit();
			}

			_unitOfWork.Flush();
		}
	}
}
