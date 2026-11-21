using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Security.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class GrantDoctorAppointmentEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 8,
                column: "Editar",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PermisosRol",
                keyColumn: "Id",
                keyValue: 8,
                column: "Editar",
                value: false);
        }
    }
}
