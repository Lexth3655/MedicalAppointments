using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Security.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AllowDoctorsToAdjustOwnSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Crear", "Editar" },
                values: new object[] { true, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Crear", "Editar" },
                values: new object[] { false, false });
        }
    }
}
