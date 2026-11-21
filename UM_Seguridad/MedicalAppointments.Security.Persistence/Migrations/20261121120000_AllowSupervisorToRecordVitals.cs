using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Security.Persistence.Migrations;

public partial class AllowSupervisorToRecordVitals : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.UpdateData(
        table: "PermisosRol",
        keyColumn: "Id",
        keyValue: 2000028,
        columns: ["Crear", "Editar"],
        values: [true, true]);

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.UpdateData(
        table: "PermisosRol",
        keyColumn: "Id",
        keyValue: 2000028,
        columns: ["Crear", "Editar"],
        values: [false, false]);
}
