using MedicalAppointments.Core;
using MedicalAppointments.Infrastructure;
using MedicalAppointments.Persistence;
using MedicalAppointments.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCore();
builder.Services.AddPersistence();
builder.Services.AddExternals();

var app = builder.Build();

// Crea la base de datos y aplica las migraciones pendientes al iniciar la API.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
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
