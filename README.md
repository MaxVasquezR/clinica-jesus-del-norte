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
├── Controllers/     # Endpoints HTTP (REST)
├── Services/        # Lógica de negocio + interfaces
├── Models/
│   ├── Entities/    # Dominio (Persona → Paciente/Médico, Cita, etc.)
│   ├── DTOs/        # Contratos de entrada/salida + validaciones
│   ├── Enums/       # Estados y catálogos tipados
│   └── Common/      # ApiResponse, PagedResult, Paginador, mensajes de validación
├── Repository/      # Persistencia en memoria (List<T>)
├── Middleware/      # Manejo centralizado de errores y validación
└── Exceptions/      # NotFound, Conflict, Business
```

## Endpoints principales

### Pacientes — `/api/pacientes`

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/pacientes?page=0&size=20` | Listado paginado |
| GET | `/api/pacientes/{id}` | Detalle |
| POST | `/api/pacientes` | Registrar |
| PUT | `/api/pacientes/{id}` | Actualizar |
| DELETE | `/api/pacientes/{id}` | Baja lógica |

### Médicos — `/api/medicos`

CRUD completo (misma forma que pacientes).

### Especialidades — `/api/especialidades`

CRUD completo.

### Citas — `/api/citas`

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/citas?page=0&size=20` | Listado (filtros: `estado`, `medicoId`, `pacienteId`) |
| GET | `/api/citas/{id}` | Detalle |
| POST | `/api/citas` | Reservar cita |
| PUT | `/api/citas/{id}` | Actualizar |
| PATCH | `/api/citas/{id}/estado` | Cambio de estado controlado |
| DELETE | `/api/citas/{id}` | Cancelar (requiere body con observación) |
| POST/GET | `/api/citas/{id}/triaje` | Triaje |
| POST/GET/PUT | `/api/citas/{id}/atencion` | Atención médica |
| POST/GET | `/api/citas/{id}/pagos` | Pago |

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

Códigos HTTP usados: `200`, `201`, `400`, `404`, `409`, `422`, `500`.

## Datos de demostración (seed)

Al iniciar la API ya existen:

- 3 especialidades (Cardiología, Pediatría, Traumatología)
- 3 médicos
- 2 pacientes
- 1 cita demo (`CJN-2026-0001`, estado `Confirmada`)

## Ejemplo rápido (cURL)

```bash
# Crear paciente
curl -X POST http://localhost:8080/api/pacientes \
  -H "Content-Type: application/json" \
  -d "{\"tipoDocumento\":\"DNI\",\"numeroDocumento\":\"40000005\",\"nombres\":\"Ana Lucía\",\"apellidos\":\"Quispe Ramos\",\"tipoSeguro\":\"Particular\",\"activo\":true}"

# Listar citas
curl "http://localhost:8080/api/citas?page=0&size=20"

# Cambiar estado
curl -X PATCH http://localhost:8080/api/citas/1/estado \
  -H "Content-Type: application/json" \
  -d "{\"estado\":\"EnTriaje\"}"
```

## Pruebas con Postman

1. Importar la colección: [`postman/CitaSync_API_Tests.json`](postman/CitaSync_API_Tests.json)
2. Verificar variable `baseUrl` = `http://localhost:8080`
3. Ejecutar la colección (Runner) o cada request individualmente
4. Para repetir la colección completa, reiniciar antes la API: las pruebas cambian los datos en memoria

Casos incluidos: creación de paciente, datos inválidos (400), recurso inexistente (404), documento duplicado (409), listado de citas, cambio de estado, transición no permitida (422), reglas de negocio de la cita, cancelación y triaje.

## Notas académicas

- Sin base de datos: los datos viven solo mientras el proceso está en ejecución.
- Validaciones con Data Annotations (`[Required]`, `[StringLength]`, `[Range]`, `[EmailAddress]`, `[EnumDataType]`).
- Excepciones centralizadas en `ExceptionMiddleware`.
- Interfaces + DTOs + herencia OOP (`Persona` → `Paciente` / `Medico`).
