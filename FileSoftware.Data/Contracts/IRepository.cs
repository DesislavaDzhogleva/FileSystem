using System.Linq.Expressions;

namespace FileSoftware.Data.Contracts
{
    public interface IRepository<TEntity> : IDisposable
       where TEntity : class
    {
        IQueryable<TEntity> All();

        IQueryable<TEntity> AllAsNoTracking();

        Task AddAsync(TEntity entity);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

        Task<int> SaveChangesAsync();
    }
}
