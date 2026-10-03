using MedicalAppointments.Patients.Core.Interfaces.Repositorios;
using MedicalAppointments.Patients.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using MedicalAppointments.Patients.Core.Feature.Patients.Queries;
using NugetClass.Pagination.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Patients.Api.Controllers
{
    [ApiController]
    [Route("c")]
    public sealed class PacientesController(IPaciente patientsRepository, IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<Paciente>>> GetAll(
            [FromQuery] RequestParameters request,
            CancellationToken cancellationToken)
        {
            var query = new GetPacientesQuery
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Filter = request.Filter,
                SortBy = request.SortBy
            };

            try
            {
                return Ok(await mediator.Send(query, cancellationToken));
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ProblemDetails { Detail = ex.Message, Status = StatusCodes.Status400BadRequest });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails { Detail = ex.Message, Status = StatusCodes.Status400BadRequest });
            }
        }

        [HttpGet("one")]
        public async Task<ActionResult<Paciente>> GetOneBy([FromQuery] string filter, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return BadRequest(new ProblemDetails { Detail = "El filtro es obligatorio.", Status = StatusCodes.Status400BadRequest });

            try
            {
                var paciente = await patientsRepository.GetOneByAsync(filter, cancellationToken);
                return paciente is null ? NotFound() : Ok(paciente);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ProblemDetails { Detail = ex.Message, Status = StatusCodes.Status400BadRequest });
            }
        }

        [HttpPost("range")]
        public async Task<IActionResult> SaveRange([FromBody] List<Paciente>? pacientes, CancellationToken cancellationToken)
        {
            if (pacientes is null || pacientes.Count == 0)
                return BadRequest(new ProblemDetails { Detail = "Envía al menos un paciente.", Status = StatusCodes.Status400BadRequest });

            if (pacientes.Any(p => p is null || string.IsNullOrWhiteSpace(p.CodigoPaciente)
                                || string.IsNullOrWhiteSpace(p.TipoDocumento)
                                || string.IsNullOrWhiteSpace(p.NumeroDocumento)
                                || string.IsNullOrWhiteSpace(p.Nombres)
                                || string.IsNullOrWhiteSpace(p.Apellidos)
                                || string.IsNullOrWhiteSpace(p.Sexo)))
                return BadRequest(new ProblemDetails { Detail = "Cada paciente debe incluir código, documento, nombres, apellidos y sexo.", Status = StatusCodes.Status400BadRequest });

            if (pacientes.GroupBy(p => p.CodigoPaciente, StringComparer.OrdinalIgnoreCase).Any(group => group.Count() > 1)
                || pacientes.GroupBy(p => $"{p.TipoDocumento}|{p.NumeroDocumento}", StringComparer.OrdinalIgnoreCase).Any(group => group.Count() > 1))
                return Conflict(new ProblemDetails { Detail = "La lista contiene códigos o documentos duplicados.", Status = StatusCodes.Status409Conflict });

            foreach (var paciente in pacientes)
            {
                paciente.PacienteId = 0;
                paciente.FechaRegistro = DateTime.UtcNow;
                paciente.ContactosEmergencia = [];
            }

            try
            {
                var savedCount = await patientsRepository.SaveRangeAsync(pacientes, cancellationToken);
                return StatusCode(StatusCodes.Status201Created, new { insertedCount = savedCount });
            }
            catch (DbUpdateException)
            {
                return Conflict(new ProblemDetails { Detail = "Uno o más códigos o documentos ya existen en la base de datos.", Status = StatusCodes.Status409Conflict });
            }
        }

        [HttpGet("{pacienteId:long}")]
        public async Task<ActionResult<Paciente>> GetById(long pacienteId, CancellationToken cancellationToken)
        {
            var paciente = await patientsRepository.GetByIdAsync(pacienteId, cancellationToken);
            if (paciente == null || !await patientsRepository.ExistsAsync(p => p.PacienteId == pacienteId, cancellationToken))
                return NotFound();
            return Ok(paciente);
        }

        [HttpPost("Create")]
        public async Task<ActionResult<Paciente>> Create(Paciente paciente, CancellationToken cancellationToken)
        {
            //Validacion del documento del paciente para evitar duplicados
            if (await patientsRepository.ExistsAsync(p => p.TipoDocumento == paciente.TipoDocumento && p.NumeroDocumento == paciente.NumeroDocumento, cancellationToken: cancellationToken))
                return Conflict(new ProblemDetails { Detail = "Ya existe un paciente con este tipo y número de documento.", Status = StatusCodes.Status409Conflict });

            // Validación y generación automática de código
            if (string.IsNullOrWhiteSpace(paciente.CodigoPaciente))
            {
                // Si no viene código, lo generamos desde el repositorio
                paciente.CodigoPaciente = await patientsRepository.GenerateNextCodigoAsync(cancellationToken);
            }
            else
            {
                // Si viene código, validamos que no exista
                if (await patientsRepository.ExistsAsync(p => p.CodigoPaciente == paciente.CodigoPaciente, cancellationToken))
                    return Conflict(new ProblemDetails { Detail = "Ya existe un paciente con este código.", Status = StatusCodes.Status409Conflict });
            }

            paciente.PacienteId = 0;
            paciente.FechaRegistro = DateTime.UtcNow;
            await patientsRepository.AddAsync(paciente, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { pacienteId = paciente.PacienteId }, paciente);
        }

        [HttpPut("{pacienteId:long}")]
        public async Task<IActionResult> Update(long pacienteId, Paciente paciente, CancellationToken cancellationToken)
        {
            // 1. Obtener la entidad existente (ya rastreada por el contexto)
            var existing = await patientsRepository.GetByIdAsync(pacienteId, cancellationToken);
            if (existing is null)
                return NotFound();

            // 2. Validar duplicado de documento (excluyendo el propio registro)
            if (await patientsRepository.ExistsAsync(p => p.TipoDocumento == paciente.TipoDocumento
                                                          && p.NumeroDocumento == paciente.NumeroDocumento
                                                          && p.PacienteId != pacienteId, cancellationToken))
                return Conflict(new ProblemDetails
                {
                    Detail = "Ya existe un paciente con este tipo y número de documento.",
                    Status = StatusCodes.Status409Conflict
                });

            // 3. Mapear los valores del DTO (o entidad recibida) al objeto rastreado
            existing.CodigoPaciente = paciente.CodigoPaciente;
            existing.TipoDocumento = paciente.TipoDocumento;
            existing.NumeroDocumento = paciente.NumeroDocumento;
            existing.Nombres = paciente.Nombres;
            existing.Apellidos = paciente.Apellidos;
            existing.FechaNacimiento = paciente.FechaNacimiento;
            existing.Sexo = paciente.Sexo;
            existing.EstadoCivil = paciente.EstadoCivil;
            existing.Telefono = paciente.Telefono;
            existing.TelefonoSecundario = paciente.TelefonoSecundario;
            existing.Email = paciente.Email;
            existing.Direccion = paciente.Direccion;
            existing.Ciudad = paciente.Ciudad;
            existing.Pais = paciente.Pais;
            existing.Ocupacion = paciente.Ocupacion;
            existing.TipoSangre = paciente.TipoSangre;
            existing.Activo = paciente.Activo;


            // 4. Guardar cambios (el contexto ya rastrea `existing`)
            await patientsRepository.UpdateAsync(existing, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{pacienteId:long}")]
        public async Task<IActionResult> Delete(long pacienteId, CancellationToken cancellationToken)
        {

            var paciente = await patientsRepository.GetByIdAsync(pacienteId, cancellationToken);
            if (paciente is null)
                return NotFound();

            // 2. Soft Delete: marcar como inactivo
            paciente.Activo = false;
            await patientsRepository.UpdateAsync(paciente, cancellationToken);

            return NoContent();
        }


    }
}