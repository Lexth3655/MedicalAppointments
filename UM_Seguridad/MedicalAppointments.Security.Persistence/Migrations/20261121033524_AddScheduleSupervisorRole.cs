using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalAppointments.Security.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleSupervisorRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Nombre" },
                values: new object[] { 4, "SupervisorTecnico" });

            migrationBuilder.InsertData(
                table: "PermisosRol",
                columns: new[] { "Id", "Crear", "Editar", "Eliminar", "Leer", "Modulo", "RolId" },
                values: new object[,]
                {
                    { 2000004, false, false, false, true, "dashboard", 4 },
                    { 2000007, false, false, false, false, "patients", 4 },
                    { 2000010, false, false, false, false, "agenda-appointments", 4 },
                    { 2000013, false, false, false, true, "agenda-doctors", 4 },
                    { 2000016, false, false, false, false, "agenda-specialties", 4 },
                    { 2000019, false, false, false, true, "agenda-clinics", 4 },
                    { 2000022, true, true, false, true, "agenda-schedules", 4 },
                    { 2000025, false, false, false, false, "clinical-attentions", 4 },
                    { 2000028, false, false, false, false, "clinical-vitals", 4 },
                    { 2000031, false, false, false, false, "clinical-notes", 4 },
                    { 2000034, false, false, false, false, "clinical-medicines", 4 },
                    { 2000037, false, false, false, false, "clinical-prescriptions", 4 },
                    { 2000040, false, false, false, false, "security-users", 4 },
                    { 2000043, false, false, false, false, "security-roles", 4 },
                    { 2000046, false, false, false, false, "security-audit", 4 },
                    { 3000004, false, false, false, false, "patients-emergency-contacts", 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000004);

            migrationBuilder.DeleteData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000007);

            migrationBuilder.DeleteData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000010);

            migrationBuilder.DeleteData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000013);

            migrationBuilder.DeleteData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000016);

            migrationBuilder.DeleteData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000019);

            migrationBuilder.DeleteData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000022);

            migrationBuilder.DeleteData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000025);

            migrationBuilder.DeleteData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000028);

            migrationBuilder.DeleteData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000031);

            migrationBuilder.DeleteData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000034);

            migrationBuilder.DeleteData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000037);

            migrationBuilder.DeleteData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000040);

            migrationBuilder.DeleteData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000043);

            migrationBuilder.DeleteData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000046);

            migrationBuilder.DeleteData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 3000004);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
