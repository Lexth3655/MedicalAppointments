using MediatR;
using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Pagination.Filters;
using NugetClass.Pagination.Models;
using System.Linq.Expressions;

namespace MedicalAppointments.Agenda.Core.Feature.Cita.Queries;

public sealed class ListarCitasQuery : RequestParameters, IRequest<PagedResult<Citas>>
{
    public long? MedicoId { get; set; }
    public long? PacienteId { get; set; }
    public long? ConsultorioId { get; set; }
    public DateOnly? Fecha { get; set; }
    public string? Estado { get; set; }
}

public sealed class ListarCitasQueryHandler(IPaginationCitas repository)
    : IRequestHandler<ListarCitasQuery, PagedResult<Citas>>
{
    public Task<PagedResult<Citas>> Handle(ListarCitasQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Citas, bool>> filter = x => x.Activo &&
            (!request.MedicoId.HasValue || x.MedicoId == request.MedicoId.Value) &&
            (!request.PacienteId.HasValue || x.PacienteId == request.PacienteId.Value) &&
            (!request.ConsultorioId.HasValue || x.ConsultorioId == request.ConsultorioId.Value) &&
            (!request.Fecha.HasValue || (x.FechaHora >= request.Fecha.Value.ToDateTime(TimeOnly.MinValue) &&
                x.FechaHora < request.Fecha.Value.AddDays(1).ToDateTime(TimeOnly.MinValue))) &&
            (string.IsNullOrWhiteSpace(request.Estado) || x.Estado == request.Estado);
        if (!string.IsNullOrWhiteSpace(request.Filter))
            filter = Combine(filter, Filter.FromStringExpression<Citas>(request.Filter));
        return repository.GetPageResponseAsync(request.PageNumber, request.PageSize, filter,
            includes: "Medico.Especialidad,Consultorio", sortBy: request.SortBy,
            asNoTracking: true, cancellationToken: cancellationToken);
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
