using System.Text;
using MedicalAppointments.Security.Core;
using MedicalAppointments.Security.Api.Middleware;
using MedicalAppointments.Security.Core.Contracts;
using MedicalAppointments.Security.Domain.Entities;
using MedicalAppointments.Security.Infrastructure;
using MedicalAppointments.Security.Infrastructure.Services;
using MedicalAppointments.Security.Persistence;
using MedicalAppointments.Security.Persistence.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddControllers();
var frontendOrigins = builder.Configuration.GetSection("Frontend:Origins").Get<string[]>()
    ?? new[] { "http://localhost:5173", "http://127.0.0.1:5173" };
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
    .WithOrigins(frontendOrigins).AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddOpenApi();
builder.Services.AddCore(builder.Configuration);
builder.Services.AddSecurityPersistence(builder.Configuration);
builder.Services.AddSecurityInfrastructure(builder.Configuration);

var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidIssuer = jwt.Issuer,
        ValidateAudience = true, ValidAudience = jwt.Audience,
        ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
        ValidateLifetime = true, ClockSkew = TimeSpan.FromSeconds(30),
        NameClaimType = System.Security.Claims.ClaimTypes.Name,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SecurityDbContext>();
    await db.Database.MigrateAsync();
    await BootstrapAdminAsync(scope.ServiceProvider, builder.Configuration);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseMiddleware<AuditMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();

static async Task BootstrapAdminAsync(IServiceProvider services, IConfiguration configuration)
{
    var username = configuration["BootstrapAdmin:Username"];
    var password = configuration["BootstrapAdmin:Password"];
    if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password)) return;
    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || password.Length < 12)
        throw new InvalidOperationException("Configure BootstrapAdmin:Username y una contraseña de al menos 12 caracteres, o quite ambas opciones.");

    var repository = services.GetRequiredService<IUsuarioRepository>();
    var existingUsers = await repository.ListarAsync(CancellationToken.None);
    if (existingUsers.Count > 0) return;
    var user = new Usuario { Username = username.Trim().ToLowerInvariant(), Nombre = configuration["BootstrapAdmin:Nombre"] ?? "Administrador", RolId = 3 };
    user.PasswordHash = services.GetRequiredService<IPasswordService>().Hash(user, password);
    await repository.AgregarAsync(user, CancellationToken.None);
    await repository.GuardarCambiosAsync(CancellationToken.None);
}


