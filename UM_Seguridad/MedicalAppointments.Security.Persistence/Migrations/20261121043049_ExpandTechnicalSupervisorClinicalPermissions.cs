using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Security.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExpandTechnicalSupervisorClinicalPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000025,
                column: "Leer",
                value: true);

            migrationBuilder.UpdateData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000028,
                column: "Leer",
                value: true);

            migrationBuilder.UpdateData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000031,
                column: "Leer",
                value: true);

            migrationBuilder.UpdateData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000034,
                columns: new[] { "Crear", "Editar", "Leer" },
                values: new object[] { true, true, true });

            migrationBuilder.UpdateData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000037,
                column: "Leer",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000025,
                column: "Leer",
                value: false);

            migrationBuilder.UpdateData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000028,
                column: "Leer",
                value: false);

            migrationBuilder.UpdateData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000031,
                column: "Leer",
                value: false);

            migrationBuilder.UpdateData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000034,
                columns: new[] { "Crear", "Editar", "Leer" },
                values: new object[] { false, false, false });

            migrationBuilder.UpdateData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 2000037,
                column: "Leer",
                value: false);
        }
    }
}
