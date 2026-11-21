# Guía para ejecutar MedicalAppointments

## Estructura

El repositorio mantiene cada microservicio como una API independiente, con capas `Domain`, `Core`, `Persistence`, `Infrastructure` y `Api`. Seguridad sigue el mismo patrón y usa MediatR en `Core` para separar controladores, casos de uso y persistencia.

Los microservicios no se combinan en un único proceso. El perfil `MedicalAppointments.slnLaunch` permite iniciar simultáneamente Patient, Agenda, Security y ClinicalCare desde Visual Studio. Las llamadas entre APIs continúan siendo HTTP.

## Iniciar los servicios desde Visual Studio

1. Abre `MedicalAppointments.slnx` con Visual Studio 2022 17.11 o posterior.
2. Si no aparece el perfil compartido en la barra superior, ve a **Tools > Options > Environment > Preview Features** y activa **Enable Multi-Project Launch Profiles**.
3. Selecciona el perfil **MedicalAppointments - Servicios locales**.
4. Presiona `F5`. El perfil inicia las cuatro APIs web: Patient, Agenda, Security y ClinicalCare.

Patient escucha en `http://127.0.0.1:5259`, Agenda en `http://127.0.0.1:5260`, Security en `http://127.0.0.1:5261` y ClinicalCare en `http://127.0.0.1:5262`. Cada API aplica sus migraciones al iniciar. Debe estar disponible la instancia de SQL Server configurada en los `appsettings`.

En Development, Scalar/OpenAPI está habilitado por servicio. Abre `/scalar/v1` en el puerto de cada API. Antes de consultar el frontend, verifica que los cuatro servicios respondan; Vite muestra `ECONNREFUSED` si una API aún no inició o se detuvo.

## Primer usuario de Security

Security no guarda una contraseña inicial en el repositorio. Antes del primer inicio, define las credenciales de bootstrap usando User Secrets desde una terminal en la carpeta del repositorio:

```powershell
$api = '.\UM_Seguridad\MedicalAppointments.Security.Api\MedicalAppointments.Security.Api.csproj'
dotnet user-secrets init --project $api
dotnet user-secrets set 'BootstrapAdmin:Username' 'admin' --project $api
dotnet user-secrets set 'BootstrapAdmin:Password' 'CAMBIA-ESTA-POR-UNA-CONTRASENA-LARGA' --project $api
dotnet user-secrets set 'BootstrapAdmin:Nombre' 'Administrador' --project $api
```

Al iniciar Security, el usuario se crea una sola vez si no existen usuarios. En esta copia local ya quedó inicializado `UserSecretsId` y creado el administrador de desarrollo. No ejecutes `dotnet user-secrets init` otra vez aquí; el secreto de bootstrap solo se usa para el primer usuario y cambiarlo después no cambia el hash guardado en la base de datos. En otra computadora, ejecuta los comandos de arriba con una contraseña propia antes del primer inicio.

## Iniciar React

Visual Studio inicia las APIs. React se inicia aparte en una terminal después de que las APIs estén listas:

```powershell
cd .\UM_Frontend\medicalappointments-web
Copy-Item .env.example .env
npm install
npm run dev
```

Abre `http://127.0.0.1:5173`. En desarrollo, Vite redirige `/backend/security`, `/backend/patients`, `/backend/agenda` y `/backend/clinical-care` a sus respectivas APIs en `127.0.0.1`.

El panel incluye login JWT, resumen de actividad, listado y registro de pacientes, listado de citas y directorio del equipo médico. Los endpoints de Agenda y Patient no tienen autorización JWT implementada todavía; el login solo protege la sesión del panel y las operaciones administrativas de Security.

## Alcance funcional pendiente

- ClinicalCare contiene atenciones, signos vitales, notas, medicamentos y prescripciones.
- La bitácora recibe las peticiones de Security. Para que registre el tráfico de toda la aplicación falta integrar Patient y Agenda con el destino central de auditoría.
- Agenda aún debe validar externamente que los IDs de paciente, médico y consultorio existan antes de guardar una cita.
