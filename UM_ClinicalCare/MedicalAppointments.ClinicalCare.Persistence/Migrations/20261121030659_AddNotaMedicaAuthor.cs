using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.ClinicalCare.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNotaMedicaAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "NotasMedicas",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MedicoId",
                table: "NotasMedicas",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "NotasMedicas");

            migrationBuilder.DropColumn(
                name: "MedicoId",
                table: "NotasMedicas");
        }
    }
}
