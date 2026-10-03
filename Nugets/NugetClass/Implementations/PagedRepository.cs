using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NugetClass.Abstractions;
using NugetClass.Pagination.Models;

namespace NugetClass.Implementations;

/// <summary>
/// Repositorio genérico base. Hereda de aquí en tus microservicios.
/// No sabe nada de tu entidad: solo sabe paginar.
/// </summary>
public abstract class PagedRepository<T, TContext> : IPagedRepository<T>
    where T : class
    where TContext : DbContext
{
    protected readonly TContext Context;
    protected readonly DbSet<T> Set;

    /// <summary>
    /// Campo por defecto para ordenar cuando el cliente no envía SortBy.
    /// Cada repositorio concreto lo sobreescribe con su PK real.
    /// Ej: protected override string DefaultSortField => nameof(Paciente.PacienteId);
    /// </summary>
    protected virtual string DefaultSortField => "Id";

    protected PagedRepository(TContext context)
    {
        Context = context;
        Set = context.Set<T>();
    }

    public IQueryable<T> Query(bool asNoTracking = true)
        => asNoTracking ? Set.AsNoTracking() : Set;

    // ────────────────────────────────────────────────────────────
    // (includes por Expression)
    // ────────────────────────────────────────────────────────────
    public async Task<PagedResult<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = true,
        bool splitQuery = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "pageNumber must be 1 or greater.");
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "pageSize must be 1 or greater.");
        pageSize = Math.Min(pageSize, NugetClass.Abstractions.PaginationDefaults.MaxPageSize);

        var query = Query(asNoTracking);

        foreach (var include in includes)
            query = query.Include(include);

        if (filter != null)
            query = query.Where(filter);

        var totalRecords = await query.CountAsync(cancellationToken);

        // Orden: si el cliente no manda orderBy, usa la PK de la entidad (DefaultSortField).
        IQueryable<T> ordered = orderBy != null
            ? orderBy(query)
            : query.OrderBy(e => EF.Property<object>(e, DefaultSortField));

        IQueryable<T> paged = ordered
            .Skip((int)Math.Min((long)(pageNumber - 1) * pageSize, int.MaxValue))
            .Take(pageSize);

        if (splitQuery && includes.Length > 0)
            paged = paged.AsSplitQuery();

        var data = await paged.ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Data = data,
            TotalRecords = totalRecords,
            CurrentPage = pageNumber,
            PageSize = pageSize
        };
    }

    // ────────────────────────────────────────────────────────────
    // OVERLOAD QUE USA TU HANDLER (includes por string, soporta anidados)
    // ────────────────────────────────────────────────────────────
    public async Task<PagedResult<T>> GetPageResponseAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        string? includes = null,
        string? sortBy = null,
        bool asNoTracking = true,
        bool splitQuery = false,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "pageNumber must be 1 or greater.");
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "pageSize must be 1 or greater.");
        pageSize = Math.Min(pageSize, NugetClass.Abstractions.PaginationDefaults.MaxPageSize);

        var query = Query(asNoTracking);

        // "Details,Details.ReturnReason" → dos Include
        if (!string.IsNullOrWhiteSpace(includes))
        {
            foreach (var inc in includes.Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                query = query.Include(inc);
            }
        }

        if (filter != null)
            query = query.Where(filter);

        var totalRecords = await query.CountAsync(cancellationToken);

        // Orden: si el cliente manda sortBy, respétalo; si no, usa DefaultSortField.
        IQueryable<T> ordered = NugetClass.Pagination.Filters.Filter.ApplySort(query, sortBy, DefaultSortField);

        IQueryable<T> paged = ordered
            .Skip((int)Math.Min((long)(pageNumber - 1) * pageSize, int.MaxValue))
            .Take(pageSize);

        if (splitQuery && !string.IsNullOrWhiteSpace(includes))
            paged = paged.AsSplitQuery();

        var data = await paged.ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Data = data,
            TotalRecords = totalRecords,
            CurrentPage = pageNumber,
            PageSize = pageSize
        };
    }
}

