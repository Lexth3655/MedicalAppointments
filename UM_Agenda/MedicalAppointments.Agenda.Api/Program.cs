using MedicalAppointments.Agenda.Core;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using MedicalAppointments.Agenda.Persistence;
using MedicalAppointments.Agenda.Persistence.Data;
using MedicalAppointments.Agenda.Infrastructure;
using System.Text.Json.Serialization;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCore(builder.Configuration);

builder.Services.AddPersistence();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Crea la base de datos y aplica las migraciones pendientes al iniciar la API.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AgendaContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
