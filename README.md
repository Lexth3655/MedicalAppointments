Roberto Alexander Toloza Mendoza Carne: TM202001

## Integración REST con el microservicio de Citas

La capa Infrastructure consume el microservicio de Citas mediante `NugetPackage_Rest`. Core define `ICitasService`, las opciones y la consulta MediatR; Infrastructure implementa el contrato HTTP; API expone `GET /api/pacientes/{pacienteId}/citas`.

En desarrollo, `appsettings.Development.json` usa `http://localhost:5102` como URL de ejemplo. El servicio de Citas no está incluido en este repositorio; para completar una llamada en vivo, configura `Downstream:CitasBaseUrl` (por variable de entorno `Downstream__CitasBaseUrl`) con la URL real. La respuesta esperada es un sobre con `succeeded`, `result`, `errorCode` y `errorMessage`; `result` contiene una lista de citas con `citaId`, `pacienteId`, `fechaHora`, `especialidad` y `estado`.

La integración tiene pruebas de unidad en `UM_Paciente/MedicalAppointments.Tests`; ejecuta todos los tests con Visual Studio Test Explorer o `dotnet test MedicalAppointments.slnx`.
