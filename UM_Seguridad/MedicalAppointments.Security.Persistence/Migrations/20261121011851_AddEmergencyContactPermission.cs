using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Security.Persistence.Migrations;

public partial class AddEmergencyContactPermission : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "PermisosRol",
            columns: new[] { "Id", "RolId", "Modulo", "Leer", "Crear", "Editar", "Eliminar" },
            values: new object[,]
            {
                { 1000001, 1, "patients-emergency-contacts", true, true, false, false },
                { 1000002, 2, "patients-emergency-contacts", true, true, true, false },
                { 1000003, 3, "patients-emergency-contacts", true, true, true, true }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(table: "PermisosRol", keyColumn: "Id", keyValues: new object[] { 1000001, 1000002, 1000003 });
    }
}
