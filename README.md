# Clínica Jesús del Norte — CitaSync API

API RESTful en **ASP.NET Core (.NET 9)** para gestionar citas médicas, pacientes, médicos, especialidades, triaje, atención clínica y pagos.

Persistencia **simulada en memoria** (`List<T>`), sin base de datos, según la rúbrica del curso *Desarrollo de Servicios Web* (IDAT).

## Integrantes

- Max Ayrton Vásquez Ruiz
- Luz Gabriela Cochachin Camacllanqui
- Brayan Luis Carpio Alegre
- Alexander Navas Rivadeneyra
- Danny Daniela Relaiza Bances

**Docente:** Ing. Daniel Arias Huallullo

## Repositorio

| Campo | Valor |
|-------|--------|
| GitHub (público — acceso de lectura para el docente) | https://github.com/MaxVasquezR/clinica-jesus-del-norte |
| Colección Postman | [`postman/CitaSync_API_Tests.json`](postman/CitaSync_API_Tests.json) |

## Requisitos

- [.NET SDK 9 o 10](https://dotnet.microsoft.com/download) (probado con ambos)
- [Postman](https://www.postman.com/downloads/) (opcional, para pruebas funcionales)

## Instalación y ejecución

```bash
# 1. Clonar el repositorio
git clone https://github.com/MaxVasquezR/clinica-jesus-del-norte.git
cd clinica-jesus-del-norte

# 2. Restaurar y compilar
dotnet restore
dotnet build

# 3. Ejecutar la API
dotnet run --project src/ClinicaJesusDelNorte.API/ClinicaJesusDelNorte.API.csproj
```

En Windows, ejecuta el proyecto desde un disco local (por ejemplo, `C:`). Desde una ruta de WSL (`\\wsl.localhost\...`), la API puede quedarse sin arrancar.

Si aparece `address already in use` en el puerto 8080, la API **ya está corriendo**: abre Swagger o cierra el proceso anterior y vuelve a ejecutar.

La API queda disponible en:

| Recurso | URL |
|---------|-----|
| Base URL | `http://localhost:8080` |
| Swagger UI | `http://localhost:8080/swagger` |
| OpenAPI JSON | `http://localhost:8080/swagger/v1/swagger.json` |

## Arquitectura de capas

```
src/ClinicaJesusDelNorte.API/
├── Controllers/     # Endpoints HTTP (REST) con permisos por rol
├── Services/        # Lógica de negocio + interfaces + datos de demostración
├── Models/
│   ├── Entities/    # Las 17 entidades del JDL; todas heredan de EntidadAuditable
│   ├── DTOs/        # Contratos de entrada/salida + validaciones
│   ├── Enums/       # Estados y catálogos tipados
│   └── Common/      # ApiResponse, PagedResult, Paginador, mensajes de validación
├── Repository/      # Repositorio genérico en memoria: asigna IDs y auditoría
├── Seguridad/       # JWT, roles y usuario de la sesión
├── Middleware/      # Manejo centralizado de errores y validación
└── Exceptions/      # NotFound, Conflict, Business, Unauthorized, Forbidden
```

## Modelo de datos

El modelo está en [`docs/citasync.jdl`](docs/citasync.jdl) (17 entidades, 10 enums). Para ver el diagrama, pega el archivo en [JDL-Studio](https://start.jhipster.tech/jdl-studio/).

- **Quién usa el sistema:** solo el personal. Cada `Usuario` tiene un `Rol`: Administrador, Recepcionista, Enfermera, Medico o Cajero. El paciente no inicia sesión.
- **Auditoría en todas las tablas:** `fechaCreacion`, `usuarioCreacion`, `fechaActualizacion` y `usuarioActualizacion`. El usuario sale del token de la sesión; los datos de demostración quedan como `SISTEMA`.
- **Historial de la cita:** cada cambio de estado deja una fila en `historial_cita` con quién lo hizo.
- En el JDL los enums van en MAYÚSCULAS (`EN_TRIAJE`); en C# y en el JSON van en PascalCase (`EnTriaje`).

## Autenticación

Todos los endpoints, salvo el login, exigen un token JWT.

1. `POST /api/auth/login` con `{ "username": "recepcion", "contrasena": "CitaSync2026" }`.
2. Envía el token en la cabecera `Authorization: Bearer {token}`. En Swagger, usa el botón **Authorize**.

Tres intentos fallidos bloquean el usuario 15 minutos; Administración puede desbloquearlo antes.

| Usuario | Rol | Qué puede hacer |
|---------|-----|-----------------|
| `admin` | Administrador | Usuarios, médicos, horarios, especialidades, aseguradoras y CIE-10 |
| `recepcion` | Recepcionista | Pacientes; reservar, reprogramar, confirmar, marcar llegada y cancelar citas |
| `enfermeria` | Enfermera | Registrar el triaje |
| `cmendoza`, `jsalazar` | Medico | Atención, diagnósticos y receta de **sus** citas |
| `caja` | Cajero | Registrar, anular y reembolsar pagos |

Contraseña de todos: `CitaSync2026`. La clave de firma del token está en `appsettings.json` solo para el entorno académico.

## Endpoints principales

Las consultas (`GET`) están abiertas a todo el personal con sesión, salvo usuarios y roles (solo Administrador), pagos (Administrador, Recepcionista y Cajero) y la atención médica (Enfermera y Médico).

### Autenticación — `/api/auth`

| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/api/auth/login` | Inicia sesión y devuelve el token |
| GET | `/api/auth/perfil` | Usuario de la sesión |

### Usuarios y roles — Administrador

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/roles` | Roles del sistema |
| GET/POST | `/api/usuarios` | Listar / crear (reutiliza la persona si ya existe por documento) |
| GET/PUT | `/api/usuarios/{id}` | Detalle / cambiar rol o activar |
| PATCH | `/api/usuarios/{id}/desbloquear` | Quitar el bloqueo por intentos fallidos |
| PATCH | `/api/usuarios/{id}/contrasena` | Asignar contraseña nueva |

### Catálogos — escritura solo Administrador

| Recurso | Ruta | Notas |
|---------|------|-------|
| Especialidades | `/api/especialidades` | CRUD; cambiar la tarifa no altera las citas ya reservadas |
| Aseguradoras | `/api/aseguradoras` | CRUD |
| CIE-10 | `/api/cie10?buscar=` | CRUD con búsqueda por código o descripción |
| Médicos | `/api/medicos?especialidadId=` | CRUD; un médico puede tener varias especialidades |
| Horarios | `/api/medicos/{id}/horarios` | Bloques semanales HH:mm sin cruces |

Las bajas (`DELETE`) son lógicas: ponen `activo = false`.

### Pacientes — `/api/pacientes` (escritura: Recepcionista)

CRUD con baja lógica. Los datos personales van en el objeto `persona`.

### Citas — `/api/citas` (escritura: Recepcionista)

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/citas?page=0&size=20` | Listado (filtros: `estado`, `medicoId`, `pacienteId`) |
| GET | `/api/citas/{id}` | Detalle |
| GET | `/api/citas/{id}/historial` | Cambios de estado con su usuario |
| POST | `/api/citas` | Reservar (estado `Solicitada`, dentro del horario del médico y sin cruces) |
| PUT | `/api/citas/{id}` | Reprogramar o corregir el motivo |
| PATCH | `/api/citas/{id}/estado` | Confirmar, `NoAsistio` o cancelar |
| PATCH | `/api/citas/{id}/llegada` | Registrar la llegada del paciente |
| DELETE | `/api/citas/{id}` | Cancelar (requiere body con observación) |

### Flujo clínico — `/api/citas/{citaId}`

| Método | Ruta | Rol | Efecto en la cita |
|--------|------|-----|-------------------|
| POST | `/triaje` | Enfermera | `Confirmada` → `EnTriaje` (requiere llegada) |
| POST | `/atencion` | Médico asignado | `EnTriaje` → `EnAtencion` |
| POST | `/atencion/diagnosticos` | Médico asignado | El primero queda como principal |
| PATCH | `/atencion/diagnosticos/{id}/principal` | Médico asignado | Cambia el diagnóstico principal |
| POST | `/atencion/receta` | Médico asignado | Agrega un medicamento |
| PUT | `/atencion` | Médico asignado | Con `estado: Completada` (y un diagnóstico principal) → `Finalizada` |
| GET/POST | `/pagos` | Cajero | Varios pagos por cita, pero solo uno `Pagado` vigente |
| PATCH | `/pagos/{pagoId}/estado` | Cajero | `Anulado` o `Reembolsado`, con motivo |

## Formato de respuesta

```json
{
  "success": true,
  "message": "Operación exitosa",
  "data": { }
}
```

Cuando hay un error, `success` es `false` y `errors` lista los problemas de validación:

```json
{
  "success": false,
  "message": "Error de validación de datos de entrada.",
  "errors": ["El campo Nombres debe tener entre 2 y 100 caracteres."]
}
```

Códigos HTTP usados: `200`, `201`, `400`, `401`, `403`, `404`, `409`, `422`, `500`.

## Datos de demostración (seed)

Al iniciar la API ya existen, auditados como `SISTEMA`:

- 5 roles y 6 usuarios (tabla de Autenticación)
- 3 especialidades (Cardiología, Pediatría, Traumatología), 3 aseguradoras y 8 códigos CIE-10
- 3 médicos con horario de lunes a sábado (Torres, de lunes a viernes por la tarde)
- 2 pacientes
- 1 cita demo (`CJN-2026-0001`, estado `Confirmada`) para el próximo día hábil a las 10:30

## Ejemplo rápido (cURL)

```bash
# Iniciar sesión como recepción y guardar el token
TOKEN=$(curl -s -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"recepcion","contrasena":"CitaSync2026"}' | jq -r .data.token)

# Crear paciente
curl -X POST http://localhost:8080/api/pacientes \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{"persona":{"tipoDocumento":"DNI","numeroDocumento":"40000005","nombres":"Ana Lucía","apellidos":"Quispe Ramos"}}'

# Registrar la llegada del paciente de la cita 1
curl -X PATCH http://localhost:8080/api/citas/1/llegada -H "Authorization: Bearer $TOKEN"

# Ver quién hizo cada cambio de estado
curl http://localhost:8080/api/citas/1/historial -H "Authorization: Bearer $TOKEN"
```

## Pruebas con Postman

1. Importar la colección: [`postman/CitaSync_API_Tests.json`](postman/CitaSync_API_Tests.json)
2. Verificar variable `baseUrl` = `http://localhost:8080`
3. Ejecutar la colección completa con el Runner, en orden: la carpeta 00 inicia sesión con cada rol y guarda los tokens
4. Para repetirla, reiniciar antes la API: las pruebas cambian los datos en memoria

Son 59 solicitudes en 6 carpetas. Recorren el flujo completo: recepción, triaje, atención médica, pago y administración. Además prueban:

- Respuestas 401 sin sesión y 403 cuando el rol no tiene permiso.
- Que solo el médico asignado pueda atender la cita.
- Reglas de horario y cruces de citas.
- Pagos anulados y facturas.
- Que cada registro guarde el usuario que lo creó o modificó.

## Notas académicas

- Sin base de datos: los datos viven solo mientras el proceso está en ejecución.
- Validaciones con Data Annotations (`[Required]`, `[StringLength]`, `[Range]`, `[RegularExpression]`, `[EnumDataType]`) e `IValidatableObject` para reglas entre campos.
- Excepciones centralizadas en `ExceptionMiddleware`.
- El repositorio genérico es el único que llena los campos de auditoría, así ninguna tabla puede quedar sin ellos.
- Contraseñas guardadas como hash (`PasswordHasher` de ASP.NET Core); la API nunca las devuelve.
- La [exportación de Swagger](docs/swagger.json) corresponde a esta versión de la API.
