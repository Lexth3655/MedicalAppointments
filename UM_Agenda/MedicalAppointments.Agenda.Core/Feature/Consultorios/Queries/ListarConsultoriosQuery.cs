using MediatR;
using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Pagination.Filters;
using NugetClass.Pagination.Models;
using System.Linq.Expressions;

namespace MedicalAppointments.Agenda.Core.Feature.Consultorio.Queries;

public sealed class ListarConsultoriosQuery : RequestParameters, IRequest<PagedResult<Consultorios>> { }

public sealed class ListarConsultoriosQueryHandler(IPaginationConsultorios repository)
    : IRequestHandler<ListarConsultoriosQuery, PagedResult<Consultorios>>
{
    public Task<PagedResult<Consultorios>> Handle(ListarConsultoriosQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Consultorios, bool>> filter = x => x.Activo;
        if (!string.IsNullOrWhiteSpace(request.Filter))
            filter = Combine(filter, Filter.FromStringExpression<Consultorios>(request.Filter));
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
