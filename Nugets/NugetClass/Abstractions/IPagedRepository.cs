using NugetClass.Pagination.Models;
using System.Linq.Expressions;


namespace NugetClass.Abstractions
{
    public interface IPagedRepository<T> where T : class
    {
        Task<PagedResult<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = true,
        bool splitQuery = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);

        // Overload extra que usa tu handler: includes como string (soporta anidados)
        Task<PagedResult<T>> GetPageResponseAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            string? includes = null,
            string? sortBy = null,
            bool asNoTracking = true,
            bool splitQuery = false,
            CancellationToken cancellationToken = default);

        IQueryable<T> Query(bool asNoTracking = true);
    }
}
