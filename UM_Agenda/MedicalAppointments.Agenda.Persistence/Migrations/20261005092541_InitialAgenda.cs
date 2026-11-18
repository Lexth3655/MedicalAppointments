using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Agenda.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAgenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Consultorios",
                schema: "dbo",
                columns: table => new
                {
                    ConsultorioID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ubicacion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EdificioNivel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Capacidad = table.Column<int>(type: "int", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultorios", x => x.ConsultorioID);
                });

            migrationBuilder.CreateTable(
                name: "Especialidades",
                schema: "dbo",
                columns: table => new
                {
                    EspecialidadID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especialidades", x => x.EspecialidadID);
                });

            migrationBuilder.CreateTable(
                name: "Medicos",
                schema: "dbo",
                columns: table => new
                {
                    MedicoID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CodigoMedico = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    EspecialidadID = table.Column<long>(type: "bigint", nullable: false),
                    Telefono = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    CorreoElectronico = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    NumeroDocumento = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TipoDocumento = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicos", x => x.MedicoID);
                    table.ForeignKey(
                        name: "FK_Medicos_Especialidades_EspecialidadID",
                        column: x => x.EspecialidadID,
                        principalSchema: "dbo",
                        principalTable: "Especialidades",
                        principalColumn: "EspecialidadID");
                });

            migrationBuilder.CreateTable(
                name: "Citas",
                schema: "dbo",
                columns: table => new
                {
                    CitaID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PacienteID = table.Column<long>(type: "bigint", nullable: false),
                    MedicoID = table.Column<long>(type: "bigint", nullable: false),
                    ConsultorioID = table.Column<long>(type: "bigint", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MotivoConsulta = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UsuarioReserva = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citas", x => x.CitaID);
                    table.ForeignKey(
                        name: "FK_Citas_Consultorios_ConsultorioID",
                        column: x => x.ConsultorioID,
                        principalSchema: "dbo",
                        principalTable: "Consultorios",
                        principalColumn: "ConsultorioID");
                    table.ForeignKey(
                        name: "FK_Citas_Medicos_MedicoID",
                        column: x => x.MedicoID,
                        principalSchema: "dbo",
                        principalTable: "Medicos",
                        principalColumn: "MedicoID");
                });

            migrationBuilder.CreateTable(
                name: "HorariosMedicos",
                schema: "dbo",
                columns: table => new
                {
                    HorarioMedicoID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicoID = table.Column<long>(type: "bigint", nullable: false),
                    ConsultorioID = table.Column<long>(type: "bigint", nullable: false),
                    FechaDisponible = table.Column<DateOnly>(type: "date", nullable: false),
                    HoraInicio = table.Column<TimeOnly>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeOnly>(type: "time", nullable: false),
                    DuracionCitaMin = table.Column<int>(type: "int", nullable: false, defaultValue: 30),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosMedicos", x => x.HorarioMedicoID);
                    table.ForeignKey(
                        name: "FK_HorariosMedicos_Consultorios_ConsultorioID",
                        column: x => x.ConsultorioID,
                        principalSchema: "dbo",
                        principalTable: "Consultorios",
                        principalColumn: "ConsultorioID");
                    table.ForeignKey(
                        name: "FK_HorariosMedicos_Medicos_MedicoID",
                        column: x => x.MedicoID,
                        principalSchema: "dbo",
                        principalTable: "Medicos",
                        principalColumn: "MedicoID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Citas_ConsultorioID",
                schema: "dbo",
                table: "Citas",
                column: "ConsultorioID");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_Medico_Fecha",
                schema: "dbo",
                table: "Citas",
                columns: new[] { "MedicoID", "FechaHora" });

            migrationBuilder.CreateIndex(
                name: "IX_Citas_Paciente_Fecha",
                schema: "dbo",
                table: "Citas",
                columns: new[] { "PacienteID", "FechaHora" });

            migrationBuilder.CreateIndex(
                name: "UX_Consultorios_Nombre",
                schema: "dbo",
                table: "Consultorios",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Especialidades_Nombre",
                schema: "dbo",
                table: "Especialidades",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HorariosMedicos_ConsultorioID",
                schema: "dbo",
                table: "HorariosMedicos",
                column: "ConsultorioID");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosMedicos_Medico_Fecha",
                schema: "dbo",
                table: "HorariosMedicos",
                columns: new[] { "MedicoID", "FechaDisponible" });

            migrationBuilder.CreateIndex(
                name: "IX_Medicos_EspecialidadID",
                schema: "dbo",
                table: "Medicos",
                column: "EspecialidadID");

            migrationBuilder.CreateIndex(
                name: "UX_Medicos_Codigo",
                schema: "dbo",
                table: "Medicos",
                column: "CodigoMedico",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Citas",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "HorariosMedicos",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Consultorios",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Medicos",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Especialidades",
                schema: "dbo");
        }
    }
}
