using System.Linq.Expressions;

namespace NugetClass.Abstractions
{
    public interface IRepository<TEntity> where TEntity : class
    {
        // Consultas
        Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<TEntity?> GetOneByAsync(string filter, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<TEntity>> GetByFilterAsync(
            Expression<Func<TEntity, bool>> filter,
            Expression<Func<TEntity, object>>? orderBy = null,
            bool ascending = true,
            CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        // Comandos
        Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken);
        Task<int> SaveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    }
}
