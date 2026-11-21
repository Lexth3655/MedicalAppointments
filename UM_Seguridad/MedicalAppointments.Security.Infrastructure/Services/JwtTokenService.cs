using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MedicalAppointments.Security.Core.Contracts;
using MedicalAppointments.Security.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MedicalAppointments.Security.Infrastructure.Services;

internal sealed class JwtTokenService(IOptions<JwtOptions> options) : ITokenService
{
    public (string Token, DateTime ExpiraUtc) CrearToken(Usuario usuario)
    {
        var settings = options.Value;
        var expires = DateTime.UtcNow.AddMinutes(settings.ExpirationMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Username),
            new("nombre", usuario.Nombre),
            new(ClaimTypes.Role, usuario.Rol.Nombre)
        };
        if (usuario.MedicoId.HasValue) claims.Add(new Claim("medicoId", usuario.MedicoId.Value.ToString()));
        foreach (var permiso in usuario.Rol.Permisos)
        {
            if (permiso.Leer) claims.Add(new Claim("perm", $"{permiso.Modulo}:read"));
            if (permiso.Crear) claims.Add(new Claim("perm", $"{permiso.Modulo}:create"));
            if (permiso.Editar) claims.Add(new Claim("perm", $"{permiso.Modulo}:edit"));
            if (permiso.Eliminar) claims.Add(new Claim("perm", $"{permiso.Modulo}:delete"));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
        var token = new JwtSecurityToken(settings.Issuer, settings.Audience, claims,
            expires: expires, signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}
