# MedicalAppointments

Sistema de gestión clínica organizado en microservicios y una interfaz React.

## Servicios y puertos

| Servicio | Puerto HTTP | Responsabilidad |
| --- | ---: | --- |
| Pacientes | 5259 | Expedientes y contactos de emergencia |
| Agenda | 5260 | Médicos, especialidades, consultorios, horarios y citas |
| Seguridad | 5261 | Inicio de sesión JWT, usuarios, roles y bitácora de auditoría |
| Atención clínica | 5262 | Atenciones, signos vitales, notas, medicamentos y prescripciones |
| Frontend React | 5173 | Panel web con menús de módulos y mantenimientos |

## Iniciar desde Visual Studio

Abre `MedicalAppointments.slnx` y selecciona el perfil de inicio **MedicalAppointments - Servicios locales**. Este inicia las cuatro APIs. Inicia React en una terminal separada:

```powershell
cd UM_Frontend/medicalappointments-web
npm install
npm run dev
```

Abre `http://127.0.0.1:5173`. El frontend enruta las llamadas a los cuatro puertos mediante el proxy de Vite. Si Visual Studio ya tenía abierta la solución, ciérrala y vuelve a abrirla para que cargue el perfil de inicio actualizado.

## Base de datos

Atención clínica usa la cadena `ConnectionStrings:ClinicalCare` de `UM_ClinicalCare/MedicalAppointments.ClinicalCare.Api/appsettings.json` y una base `MedicalAppointments_ClinicalCare` en SQL Server local. Al arrancar, aplica la migración inicial automáticamente. Ajusta esa cadena si tu instancia de SQL Server no es `.`.

Al crear una atención, el servicio valida el ID de cita, paciente y médico contra Agenda en `http://localhost:5260`. Se puede cambiar la URL con `Downstream__AgendaBaseUrl`.

## Módulos web

- Inicio y resumen de actividad.
- Pacientes: alta, consulta, edición y desactivación.
- Agenda: citas, médicos, especialidades, consultorios y horarios.
- Atención clínica: atenciones, signos vitales, notas médicas, catálogo de medicamentos y prescripciones.
- Administración: alta y activación/desactivación de usuarios, consulta de roles y bitácora.

Los endpoints protegidos de Seguridad requieren iniciar sesión con un usuario habilitado. La cuenta inicial de administrador se configura mediante los User Secrets del proyecto de Seguridad; no se guarda una contraseña en el repositorio.
