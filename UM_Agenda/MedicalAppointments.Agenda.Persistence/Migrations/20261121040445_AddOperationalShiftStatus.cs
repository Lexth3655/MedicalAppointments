using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Agenda.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationalShiftStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EstadoTurno",
                schema: "dbo",
                table: "HorariosMedicos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Programado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoTurno",
                schema: "dbo",
                table: "HorariosMedicos");
        }
    }
}
