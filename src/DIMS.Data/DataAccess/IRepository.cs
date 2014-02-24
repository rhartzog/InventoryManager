using System.Linq;

namespace DIMS.Data.DataAccess
{
    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> AsQueryable();
        bool Delete(TEntity entity);
        void SaveChanges();
        bool Update(TEntity entity);
        bool Create(TEntity entity);
    }
}