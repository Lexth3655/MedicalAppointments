using MedicalAppointments.ClinicalCare.Core.Contracts;
using MedicalAppointments.ClinicalCare.Domain.Entities;
using MedicalAppointments.ClinicalCare.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.ClinicalCare.Persistence.Repositories;

internal sealed class ClinicalCareRepository(ClinicalCareDbContext db) : IClinicalCareRepository
{
    public Task<List<Atencion>> ListarAtencionesAsync(long? pacienteId, CancellationToken ct) =>
        db.Atenciones.AsNoTracking().Include(x => x.SignosVitales).OrderByDescending(x => x.FechaHora)
            .Where(x => !pacienteId.HasValue || x.PacienteId == pacienteId.Value).ToListAsync(ct);
    public Task<Atencion?> ObtenerAtencionAsync(long id, CancellationToken ct) =>
        db.Atenciones.Include(x => x.SignosVitales).Include(x => x.NotasMedicas).Include(x => x.Prescripciones)
            .ThenInclude(x => x.Detalles).ThenInclude(x => x.Medicamento).SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<Atencion?> ObtenerAtencionPorCitaAsync(long citaId, CancellationToken ct) => db.Atenciones.SingleOrDefaultAsync(x => x.CitaId == citaId, ct);
    public async Task AgregarAtencionAsync(Atencion atencion, CancellationToken ct) { db.Atenciones.Add(atencion); await db.SaveChangesAsync(ct); }
    public async Task GuardarAtencionAsync(Atencion atencion, CancellationToken ct) { db.Atenciones.Update(atencion); await db.SaveChangesAsync(ct); }

    public Task<List<SignosVitales>> ListarSignosVitalesAsync(long atencionId, CancellationToken ct) =>
        db.SignosVitales.AsNoTracking().Where(x => x.AtencionId == atencionId).OrderByDescending(x => x.FechaRegistroUtc).ToListAsync(ct);
    public Task<SignosVitales?> ObtenerSignosVitalesAsync(long id, CancellationToken ct) => db.SignosVitales.FindAsync(new object[] { id }, ct).AsTask();
    public async Task AgregarSignosVitalesAsync(SignosVitales signos, CancellationToken ct) { db.SignosVitales.Add(signos); await db.SaveChangesAsync(ct); }
    public async Task GuardarSignosVitalesAsync(SignosVitales signos, CancellationToken ct) { db.SignosVitales.Update(signos); await db.SaveChangesAsync(ct); }

    public Task<List<NotaMedica>> ListarNotasAsync(long atencionId, CancellationToken ct) =>
        db.NotasMedicas.AsNoTracking().Where(x => x.AtencionId == atencionId).OrderByDescending(x => x.FechaHora).ToListAsync(ct);
    public Task<NotaMedica?> ObtenerNotaAsync(long id, CancellationToken ct) => db.NotasMedicas.FindAsync(new object[] { id }, ct).AsTask();
    public async Task AgregarNotaAsync(NotaMedica nota, CancellationToken ct) { db.NotasMedicas.Add(nota); await db.SaveChangesAsync(ct); }
    public async Task GuardarNotaAsync(NotaMedica nota, CancellationToken ct) { db.NotasMedicas.Update(nota); await db.SaveChangesAsync(ct); }

    public Task<List<Medicamento>> ListarMedicamentosAsync(bool incluirInactivos, CancellationToken ct) =>
        db.Medicamentos.AsNoTracking().Where(x => incluirInactivos || x.Activo).OrderBy(x => x.Nombre).ToListAsync(ct);
    public Task<Medicamento?> ObtenerMedicamentoAsync(long id, CancellationToken ct) => db.Medicamentos.FindAsync(new object[] { id }, ct).AsTask();
    public async Task AgregarMedicamentoAsync(Medicamento item, CancellationToken ct) { db.Medicamentos.Add(item); await db.SaveChangesAsync(ct); }
    public async Task GuardarMedicamentoAsync(Medicamento item, CancellationToken ct) { db.Medicamentos.Update(item); await db.SaveChangesAsync(ct); }

    public Task<List<Prescripcion>> ListarPrescripcionesAsync(long? atencionId, long? pacienteId, CancellationToken ct) =>
        db.Prescripciones.AsNoTracking().Include(x => x.Detalles).ThenInclude(x => x.Medicamento)
            .Where(x => (!atencionId.HasValue || x.AtencionId == atencionId.Value) && (!pacienteId.HasValue || x.PacienteId == pacienteId.Value))
            .OrderByDescending(x => x.Fecha).ToListAsync(ct);
    public Task<Prescripcion?> ObtenerPrescripcionAsync(long id, CancellationToken ct) =>
        db.Prescripciones.AsNoTracking().Include(x => x.Detalles).ThenInclude(x => x.Medicamento).SingleOrDefaultAsync(x => x.Id == id, ct);
    public async Task AgregarPrescripcionAsync(Prescripcion item, CancellationToken ct) { db.Prescripciones.Add(item); await db.SaveChangesAsync(ct); }
}
