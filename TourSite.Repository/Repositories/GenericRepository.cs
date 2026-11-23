using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TourSite.Core.Repositories.Contract;
using TourSite.Core.Specification;
using TourSite.Repository.Data.Contexts;

namespace TourSite.Repository.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly TourDbContext _context;

        public GenericRepository(TourDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TEntity>> GetAllSpecAsync(ISpecifications<TEntity> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<TEntity?> GetByIdSpecAsync(ISpecifications<TEntity> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<TEntity>().AddAsync(entity, cancellationToken);
        }

        public void Update(TEntity entity)
        {
            _context.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _context.Remove(entity);
        }

        public Task<int> GetCountAsync(ISpecifications<TEntity> spec)
        {
            var query = _context.Set<TEntity>().AsQueryable();
            if (spec.Criteria is not null)
                query = query.Where(spec.Criteria);
            return query.CountAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }

        public IQueryable<TEntity> Query()
        {
            return _context.Set<TEntity>().AsQueryable();
        }

        private IQueryable<TEntity> ApplySpecification(ISpecifications<TEntity> spec)
        {
            return SpecificationEvaluator<TEntity>.GetQuery(_context.Set<TEntity>(), spec);
        }
    }
}
