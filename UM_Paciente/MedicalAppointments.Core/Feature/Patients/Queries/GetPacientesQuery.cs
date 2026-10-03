using MediatR;
using MedicalAppointments.Patients.Core.Interfaces.Repositorios;
using MedicalAppointments.Patients.Domain.Models;
using NugetClass.Pagination.Filters;
using NugetClass.Pagination.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace MedicalAppointments.Patients.Core.Feature.Patients.Queries
{
    public class GetPacientesQuery : RequestParameters, IRequest<PagedResult<Paciente>>
    {
    }

    public class GetPacientesQueryHandler : IRequestHandler<GetPacientesQuery, PagedResult<Paciente>>
    {
        private readonly IPaginationPaciente _paginationRepository;

        public GetPacientesQueryHandler(IPaginationPaciente paginationRepository)
        {
            _paginationRepository = paginationRepository ?? throw new ArgumentNullException(nameof(paginationRepository));
        }

        public async Task<PagedResult<Paciente>> Handle(GetPacientesQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Paciente, bool>> activeFilter = paciente => paciente.Activo;
            if (!string.IsNullOrWhiteSpace(request.Filter))
            {
                var requestedFilter = Filter.FromStringExpression<Paciente>(request.Filter);
                activeFilter = Combine(activeFilter, requestedFilter);
            }

            return await _paginationRepository.GetPageResponseAsync(
                request.PageNumber,
                request.PageSize,
                activeFilter,
                sortBy: request.SortBy,
                asNoTracking: true,
                cancellationToken: cancellationToken);
        }

        private static Expression<Func<T, bool>> Combine<T>(
            Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(T), "entity");
            var firstBody = new ReplaceParameterVisitor(first.Parameters[0], parameter).Visit(first.Body)!;
            var secondBody = new ReplaceParameterVisitor(second.Parameters[0], parameter).Visit(second.Body)!;
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(firstBody, secondBody), parameter);
        }

        private sealed class ReplaceParameterVisitor(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
        {
            protected override Expression VisitParameter(ParameterExpression node) =>
                node == source ? target : base.VisitParameter(node);
        }
    }
}