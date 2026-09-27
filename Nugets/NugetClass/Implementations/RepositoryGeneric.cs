using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NugetClass.Abstractions;


namespace NugetClass.Implementations
{
    public class RepositoryGeneric<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public RepositoryGeneric(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        //Comandos

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await _context.Set<TEntity>().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
        {
            var entity = await _context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
                return false;

            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        //Consultas
        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }



        public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<TEntity>().ToListAsync(cancellationToken);
        }


        public async Task<IReadOnlyList<TEntity>> GetByFilterAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>>? orderBy = null, bool ascending = true, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<TEntity>().AsNoTracking().Where(filter);
            if (orderBy != null)
                query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var result = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
            return result;
        }
    }
}
