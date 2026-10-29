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
| Informe técnico (borrador PDF) | [`docs/INFORME_TECNICO.md`](docs/INFORME_TECNICO.md) |
| Guía de capturas Postman | [`docs/GUIA_CAPTURAS_POSTMAN.md`](docs/GUIA_CAPTURAS_POSTMAN.md) |

## Requisitos

- [.NET SDK 9+](https://dotnet.microsoft.com/download) (probado con SDK 10)
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

Si aparece `address already in use` en el puerto 8080, la API **ya está corriendo**: abrí Swagger o cerrá el proceso anterior y volvé a ejecutar.

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
│   └── Common/      # ApiResponse, PagedResult
├── Repository/      # Persistencia en memoria (List/Dictionary)
├── Middleware/      # Manejo centralizado de excepciones
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
  "data": { },
  "errors": null
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
  -d "{\"tipoDocumento\":\"DNI\",\"numeroDocumento\":\"48129599\",\"nombres\":\"Max Ayrton\",\"apellidos\":\"Vásquez Ruiz\",\"tipoSeguro\":\"Particular\",\"activo\":true}"

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

Casos incluidos: creación de paciente, listado de citas, cambio de estado, cancelación, conflicto de documento, 404.

## Informe técnico

Borrador listo para exportar a PDF: [`docs/INFORME_TECNICO.md`](docs/INFORME_TECNICO.md)

## Notas académicas

- Sin base de datos: los datos viven solo mientras el proceso está en ejecución.
- Validaciones con Data Annotations (`[Required]`, `[StringLength]`, `[Range]`, `[EmailAddress]`, `[EnumDataType]`).
- Excepciones centralizadas en `ExceptionMiddleware`.
- Interfaces + DTOs + herencia OOP (`Persona` → `Paciente` / `Medico`).
