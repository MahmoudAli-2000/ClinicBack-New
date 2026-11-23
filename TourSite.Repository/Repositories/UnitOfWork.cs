using System.Collections;
using System.Data;
using Microsoft.EntityFrameworkCore;
using TourSite.Core;
using TourSite.Core.Repositories.Contract;
using TourSite.Repository.Data.Contexts;

namespace TourSite.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TourDbContext _context;
        private readonly Hashtable repositiries;

        public UnitOfWork(TourDbContext context)
        {
            _context = context;
            repositiries = new Hashtable();
        }

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity).Name;

            if (!repositiries.ContainsKey(type))
            {
                var repository = new GenericRepository<TEntity>(_context);
                repositiries.Add(type, repository);
            }

            return (IGenericRepository<TEntity>)repositiries[type]!;
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
        {
            await ExecuteInTransactionAsync(async () =>
            {
                await action();
                return true;
            }, isolationLevel, cancellationToken);
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
                try
                {
                    var result = await action();
                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }
    }
}
