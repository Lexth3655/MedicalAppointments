using MedicalAppointments.Patients.Core.Interfaces.Repositorios;
using MedicalAppointments.Patients.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using NugetClass.Pagination.Filters;
using NugetClass.Pagination.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace MedicalAppointments.Patients.Api.Controllers
{
    [ApiController]
    [Route("patients/{pacienteId:long}/emergency-contacts")]
    public sealed class ContactosEmergenciaController(IPaciente patientsRepository, IContactoEmergencia contactsRepository, IPaginationContactoEmergencia paginationRepository) : ControllerBase
    {
        [HttpGet("/EmergencyContacts")]
        public async Task<ActionResult<PagedResult<ContactoEmergencia>>> GetAllContacts(
            [FromQuery] RequestParameters request,
            CancellationToken cancellationToken)
        {
            try
            {
                var filter = string.IsNullOrWhiteSpace(request.Filter)
                    ? null
                    : Filter.FromStringExpression<ContactoEmergencia>(request.Filter);
                return Ok(await paginationRepository.GetPageResponseAsync(
                    request.PageNumber, request.PageSize, filter, sortBy: request.SortBy,
                    asNoTracking: true, cancellationToken: cancellationToken));
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ProblemDetails { Detail = ex.Message, Status = StatusCodes.Status400BadRequest });
            }
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ContactoEmergencia>>> GetAll(
            long pacienteId,
            [FromQuery] RequestParameters request,
            CancellationToken cancellationToken)
        {
            if (await patientsRepository.GetByIdAsync(pacienteId, cancellationToken) is null) return NotFound();

            try
            {
                Expression<Func<ContactoEmergencia, bool>> filter = contact => contact.PacienteId == pacienteId;
                if (!string.IsNullOrWhiteSpace(request.Filter))
                {
                    var requestedFilter = Filter.FromStringExpression<ContactoEmergencia>(request.Filter);
                    filter = Combine(filter, requestedFilter);
                }

                return Ok(await paginationRepository.GetPageResponseAsync(
                    request.PageNumber, request.PageSize, filter, sortBy: request.SortBy,
                    asNoTracking: true, cancellationToken: cancellationToken));
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ProblemDetails { Detail = ex.Message, Status = StatusCodes.Status400BadRequest });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ContactoEmergencia>> Create(long pacienteId, ContactoEmergencia contacto, CancellationToken cancellationToken)
        {
            if (await patientsRepository.GetByIdAsync(pacienteId, cancellationToken) is null) return NotFound();
            contacto.ContactoEmergenciaId = 0;
            contacto.PacienteId = pacienteId;
            await contactsRepository.AddAsync(contacto, cancellationToken);
            return CreatedAtAction(nameof(GetAll), new { pacienteId }, contacto);
        }

        [HttpPut("{contactoId:long}")]
        public async Task<IActionResult> Update(long contactoId, ContactoEmergencia contacto, CancellationToken cancellationToken)
        {
            // 1. Obtener la entidad existente (ya rastreada por el contexto)
            var existing = await contactsRepository.GetByIdAsync(contactoId, cancellationToken);
            if (existing is null)
                return NotFound();

            // 2. (Opcional) Validar que el paciente exista si estás cambiando el PacienteId
            if (contacto.PacienteId != existing.PacienteId)
            {
                var pacienteExiste = await patientsRepository.ExistsAsync(p => p.PacienteId == contacto.PacienteId, cancellationToken);
                if (!pacienteExiste)
                    return Conflict(new ProblemDetails { Detail = "El paciente especificado no existe.", Status = StatusCodes.Status409Conflict });
            }

            // 3. Mapear los valores al objeto rastreado
            existing.PacienteId = contacto.PacienteId;
            existing.NombreCompleto = contacto.NombreCompleto;
            existing.Parentesco = contacto.Parentesco;
            existing.Telefono = contacto.Telefono;
            existing.TelefonoSecundario = contacto.TelefonoSecundario;
            existing.Email = contacto.Email;
            existing.Prioridad = contacto.Prioridad;
            existing.Activo = contacto.Activo;
            // No modifiques ContactoEmergenciaId

            // 4. Guardar cambios (el contexto ya rastrea `existing`)
            await contactsRepository.UpdateAsync(existing, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{contactoEmergenciaId:long}")]
        public async Task<IActionResult> Delete(long pacienteId, long contactoEmergenciaId, CancellationToken cancellationToken)
        {
            var existing = await contactsRepository.GetByIdAsync(contactoEmergenciaId, cancellationToken);
            if (existing is null)
                return NotFound();

            // Soft delete
            existing.Activo = false;
            await contactsRepository.UpdateAsync(existing, cancellationToken);

            return NoContent();
        }

        private static Expression<Func<T, bool>> Combine<T>(Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
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
