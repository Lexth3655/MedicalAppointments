using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalAppointments.Security.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSecurityWithPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auditoria",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Metodo = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Ruta = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Recurso = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Detalle = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CodigoEstado = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    DireccionIp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    AgenteUsuario = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TraceId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DuracionMs = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermisosRol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    Modulo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Leer = table.Column<bool>(type: "bit", nullable: false),
                    Crear = table.Column<bool>(type: "bit", nullable: false),
                    Editar = table.Column<bool>(type: "bit", nullable: false),
                    Eliminar = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermisosRol", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermisosRol_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    MedicoId = table.Column<long>(type: "bigint", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacionUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Recepcion" },
                    { 2, "Medico" },
                    { 3, "Administrador" }
                });

            migrationBuilder.InsertData(
                table: "PermisosRol",
                columns: new[] { "Id", "Crear", "Editar", "Eliminar", "Leer", "Modulo", "RolId" },
                values: new object[,]
                {
                    { 1, false, false, false, true, "dashboard", 1 },
                    { 2, false, false, false, true, "dashboard", 2 },
                    { 3, true, true, true, true, "dashboard", 3 },
                    { 4, true, false, false, true, "patients", 1 },
                    { 5, false, false, false, true, "patients", 2 },
                    { 6, true, true, true, true, "patients", 3 },
                    { 7, true, false, false, true, "agenda-appointments", 1 },
                    { 8, false, false, false, true, "agenda-appointments", 2 },
                    { 9, true, true, true, true, "agenda-appointments", 3 },
                    { 10, false, false, false, true, "agenda-doctors", 1 },
                    { 11, false, false, false, true, "agenda-doctors", 2 },
                    { 12, true, true, true, true, "agenda-doctors", 3 },
                    { 13, false, false, false, true, "agenda-specialties", 1 },
                    { 14, false, false, false, false, "agenda-specialties", 2 },
                    { 15, true, true, true, true, "agenda-specialties", 3 },
                    { 16, false, false, false, true, "agenda-clinics", 1 },
                    { 17, false, false, false, true, "agenda-clinics", 2 },
                    { 18, true, true, true, true, "agenda-clinics", 3 },
                    { 19, false, false, false, true, "agenda-schedules", 1 },
                    { 20, false, false, false, true, "agenda-schedules", 2 },
                    { 21, true, true, true, true, "agenda-schedules", 3 },
                    { 22, false, false, false, true, "clinical-attentions", 1 },
                    { 23, true, true, false, true, "clinical-attentions", 2 },
                    { 24, true, true, true, true, "clinical-attentions", 3 },
                    { 25, false, false, false, false, "clinical-vitals", 1 },
                    { 26, true, true, false, true, "clinical-vitals", 2 },
                    { 27, true, true, true, true, "clinical-vitals", 3 },
                    { 28, false, false, false, false, "clinical-notes", 1 },
                    { 29, true, true, false, true, "clinical-notes", 2 },
                    { 30, true, true, true, true, "clinical-notes", 3 },
                    { 31, false, false, false, false, "clinical-medicines", 1 },
                    { 32, true, true, false, true, "clinical-medicines", 2 },
                    { 33, true, true, true, true, "clinical-medicines", 3 },
                    { 34, false, false, false, false, "clinical-prescriptions", 1 },
                    { 35, true, true, false, true, "clinical-prescriptions", 2 },
                    { 36, true, true, true, true, "clinical-prescriptions", 3 },
                    { 37, false, false, false, false, "security-users", 1 },
                    { 38, false, false, false, false, "security-users", 2 },
                    { 39, true, true, true, true, "security-users", 3 },
                    { 40, false, false, false, false, "security-roles", 1 },
                    { 41, false, false, false, false, "security-roles", 2 },
                    { 42, true, true, true, true, "security-roles", 3 },
                    { 43, false, false, false, false, "security-audit", 1 },
                    { 44, false, false, false, false, "security-audit", 2 },
                    { 45, true, true, true, true, "security-audit", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auditoria_CodigoEstado_FechaUtc",
                table: "Auditoria",
                columns: new[] { "CodigoEstado", "FechaUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Auditoria_FechaUtc",
                table: "Auditoria",
                column: "FechaUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Auditoria_UsuarioId",
                table: "Auditoria",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PermisosRol_RolId_Modulo",
                table: "PermisosRol",
                columns: new[] { "RolId", "Modulo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Nombre",
                table: "Roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_MedicoId",
                table: "Usuarios",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId",
                table: "Usuarios",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Username",
                table: "Usuarios",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auditoria");

            migrationBuilder.DropTable(
                name: "PermisosRol");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
