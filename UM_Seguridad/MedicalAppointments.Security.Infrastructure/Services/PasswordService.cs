using MedicalAppointments.Security.Core.Contracts;
using MedicalAppointments.Security.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace MedicalAppointments.Security.Infrastructure.Services;

internal sealed class PasswordService : IPasswordService
{
    private readonly PasswordHasher<Usuario> _hasher = new();

    public string Hash(Usuario usuario, string password) => _hasher.HashPassword(usuario, password);

    public bool Verificar(Usuario usuario, string password) =>
        _hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, password) != PasswordVerificationResult.Failed;
}
