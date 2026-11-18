using MediatR;
using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Pagination.Filters;
using NugetClass.Pagination.Models;
using System.Linq.Expressions;

namespace MedicalAppointments.Agenda.Core.Feature.Especialidad.Queries;

public sealed class ListarEspecialidadesQuery : RequestParameters, IRequest<PagedResult<Especialidades>> { }

public sealed class ListarEspecialidadesQueryHandler(IPaginationEspecialidades repository)
    : IRequestHandler<ListarEspecialidadesQuery, PagedResult<Especialidades>>
{
    public Task<PagedResult<Especialidades>> Handle(ListarEspecialidadesQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Especialidades, bool>> filter = x => x.Activo;
        if (!string.IsNullOrWhiteSpace(request.Filter))
            filter = Combine(filter, Filter.FromStringExpression<Especialidades>(request.Filter));
        return repository.GetPageResponseAsync(request.PageNumber, request.PageSize, filter,
            sortBy: request.SortBy, asNoTracking: true, cancellationToken: cancellationToken);
    }

    private static Expression<Func<T, bool>> Combine<T>(Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(T), "entity");
        var left = new ReplaceParameterVisitor(first.Parameters[0], parameter).Visit(first.Body)!;
        var right = new ReplaceParameterVisitor(second.Parameters[0], parameter).Visit(second.Body)!;
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
    }
    private sealed class ReplaceParameterVisitor(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) => node == source ? target : base.VisitParameter(node);
    }
}
