using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Security.Persistence.Migrations;

public partial class GrantSupervisorAppointmentsRead : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.UpdateData(
        table: "PermisosRol", keyColumn: "Id", keyValue: 2000010, column: "Leer", value: true);

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.UpdateData(
        table: "PermisosRol", keyColumn: "Id", keyValue: 2000010, column: "Leer", value: false);
}
