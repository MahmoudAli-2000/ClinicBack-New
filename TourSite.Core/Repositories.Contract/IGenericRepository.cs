using System.Linq.Expressions;
using TourSite.Core.Specification;

namespace TourSite.Core.Repositories.Contract
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllSpecAsync(ISpecifications<TEntity> spec);
        Task<TEntity?> GetByIdSpecAsync(ISpecifications<TEntity> spec);
        Task<int> GetCountAsync(ISpecifications<TEntity> spec);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        IQueryable<TEntity> Query();
    }
}
