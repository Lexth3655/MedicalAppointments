# MedicalAppointments.Security

Microservicio REST de autenticación y administración básica de usuarios. Implementado en .NET 9 siguiendo las capas Domain, Core, Persistence, Infrastructure y Api.

## Configuración y primer inicio

La API usa SQL Server y una base de datos propia (`MedicalAppointments_Security`). Las migraciones iniciales crean los roles `Recepcion`, `Medico`, `Administrador` y `SupervisorTecnico`. El supervisor técnico puede consultar médicos y consultorios, y planificar o editar horarios mensuales.

Antes del primer inicio, configura estos valores como variables de entorno (doble guion bajo para separar secciones):

```powershell
$env:ConnectionStrings__Security = 'Server=.;Database=MedicalAppointments_Security;Trusted_Connection=true;TrustServerCertificate=true'
$env:Jwt__Key = 'una-clave-aleatoria-de-al-menos-32-bytes'
$env:BootstrapAdmin__Username = 'admin'
$env:BootstrapAdmin__Password = 'una-clave-inicial-de-12-caracteres-o-mas'
$env:BootstrapAdmin__Nombre = 'Administrador inicial'
dotnet run --project .\MedicalAppointments.Security.Api
```

`Jwt__Key` debe ser secreta, aleatoria y de al menos 32 bytes. No guardes la clave ni la contraseña inicial en el repositorio. El usuario bootstrap solo se crea si todavía no hay usuarios; después de iniciar sesión, el administrador puede crear las cuentas de Recepción y Médico. Quita las variables de bootstrap una vez creado el administrador.

En desarrollo, Scalar/OpenAPI se publica desde la API. La contraseña de usuario se almacena con el `PasswordHasher` de ASP.NET Core Identity; el JWT dura 60 minutos por defecto.

## Endpoints

| Método | Ruta | Acceso | Propósito |
|---|---|---|---|
| POST | `/api/auth/login` | Público | Validar credenciales y emitir JWT |
| GET | `/api/roles` | JWT | Listar roles disponibles |
| GET | `/api/usuarios` | Rol Administrador | Listar usuarios sin exponer hashes |
| GET | `/api/usuarios/{id}` | Rol Administrador | Consultar un usuario |
| POST | `/api/usuarios` | Rol Administrador | Crear usuario (contraseña mínima de 12 caracteres) |
| PATCH | `/api/usuarios/{id}/estado` | Rol Administrador | Activar/desactivar cuenta |
| GET | `/api/auditoria?pagina=1&tamano=50` | Rol Administrador | Consultar la bitácora paginada |

Ejemplo de login:

```http
POST /api/auth/login
Content-Type: application/json

{"username":"admin","password":"<contraseña>"}
```

Las demás rutas protegidas aceptan `Authorization: Bearer <token>`. Para crear un médico, `MedicoId` es obligatorio; para otros roles debe omitirse o ser `null`. `MedicoId` es un `BIGINT`, compatible con el identificador que usa actualmente Agenda. Es externo y no crea una clave foránea entre bases.

## Auditoría

El middleware escribe una fila en `dbo.Auditoria` por petición con fecha UTC, método, ruta, estado HTTP, usuario autenticado (si existe), IP, agente de usuario, TraceId y duración. Se omiten deliberadamente cuerpos, query strings, contraseñas, cabeceras de autorización y datos clínicos. El acceso de lectura de la bitácora queda restringido al rol Administrador.

La planificación compartida excluye auditoría detallada del alcance base; esta bitácora se incorpora por la petición expresa del usuario y añade una tabla (16 en total al sumarse al límite documentado de 15).
