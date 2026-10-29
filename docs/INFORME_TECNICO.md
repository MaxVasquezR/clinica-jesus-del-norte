# INFORME TÉCNICO — PRUEBAS DE CALIDAD Y DESARROLLO DE SOFTWARE

**Instituto Superior Tecnológico Privado IDAT**  
**Carrera:** Desarrollo de Sistemas Front-End y Back-End — EEST Virtual  
**Unidad didáctica:** Desarrollo de Servicios Web  
**Proyecto:** CitaSync — Clínica Jesús del Norte  
**Docente:** Ing. Daniel Arias Huallullo  
**Año:** 2026

---

## 1. INTEGRANTES

| Integrante | Rol sugerido en el desarrollo |
|---|---|
| Max Ayrton Vásquez Ruiz | Arquitectura, modelos de dominio y repositorios |
| Luz Gabriela Cochachin Camacllanqui | Servicios de negocio y validaciones |
| Brayan Luis Carpio Alegre | Controllers REST y códigos HTTP |
| Alexander Navas Rivadeneyra | Middleware de excepciones, Swagger y Postman |
| Danny Daniela Relaiza Bances | README, documentación e informe técnico |

> Ajusten los roles según la participación real del equipo antes de exportar a PDF.

**Repositorio GitHub:** https://github.com/MaxVasquezR/clinica-jesus-del-norte  
> Repositorio público con acceso de lectura para el docente.

**Colección Postman del proyecto:** `postman/CitaSync_API_Tests.json`  
**Guía paso a paso de capturas:** `docs/GUIA_CAPTURAS_POSTMAN.md`

---

## 2. DESCRIPCIÓN GENERAL Y FLUJO OPERATIVO DEL NEGOCIO

El sistema **CitaSync** automatiza el ciclo de vida de la atención médica en la **Clínica Jesús del Norte**, eliminando los cuellos de botella de la agenda física tradicional.

El flujo operativo se rige bajo 6 fases críticas validadas por el backend:

1. **Identidad del Paciente:** Registro centralizado y validación de perfiles y documentos.
2. **Configuración Médica y Agenda:** Administración de especialidades, médicos colegiados, RNE.
3. **Registro de Cita por Recepción:** Asociación entre paciente, médico, especialidad y horario.
4. **Llegada y Triaje:** Signos vitales y priorización clínica.
5. **Atención e Historia Clínica:** Diagnóstico CIE-10, evolución, tratamiento y receta.
6. **Pago y Auditoría:** Transacciones (Yape, Plin, Tarjeta, Efectivo) y trazabilidad de estados.

---

## 3. MODELADO DE DATOS (ENTIDADES C#)

| Entidad | Campos clave |
|---|---|
| Persona (base) | tipoDocumento, numeroDocumento (unique ≤15), nombres, apellidos, fechaNacimiento, telefono, email, direccion, activo |
| Paciente | codigoPaciente, tipoSeguro, numeroPoliza |
| Médico | codigoColegiatura, rne, especialidadId |
| Especialidad | nombre (unique), descripcion, precioBase ≥ 0, activo |
| CitaMedica | codigoCita, fechaHoraCita, motivoConsulta, estado, fechaHoraLlegada, observacionCancelacion |
| Triaje | signos vitales, prioridad |
| AtencionMedica | diagnosticoCIE10, evolucion, tratamiento, receta, estado |
| PagoTransaccion | codigoTransaccion, montoTotal, metodoPago, estadoPago |

**Enums:** TipoDocumento, TipoSeguro, EstadoCita, PrioridadTriaje, MetodoPago, EstadoPago, EstadoAtencion.

Se aplica **herencia OOP**: `Paciente` y `Medico` heredan de `Persona`.

---

## 4. ARQUITECTURA DEL SISTEMA

```
ClinicaJesusDelNorte.API/
├── Controllers/    # Endpoints HTTP
├── Services/       # Interfaces + lógica de negocio
├── Models/         # Entities, DTOs, Enums, ApiResponse
├── Repository/     # Persistencia en memoria (List<T>)
├── Middleware/     # ExceptionMiddleware
└── Exceptions/     # NotFound / Conflict / Business
```

- Inyección de dependencias en `Program.cs`
- DTOs con Data Annotations (`[Required]`, `[StringLength]`, `[Range]`, etc.)
- Respuestas uniformes `ApiResponse<T>`
- Persistencia en memoria thread-safe (simulación de BD)

---

## 5. ENDPOINTS IMPLEMENTADOS

### Pacientes `/api/pacientes`
- `POST` registrar · `GET` listar/detalle · `PUT` actualizar · `DELETE` baja lógica

### Médicos `/api/medicos` y Especialidades `/api/especialidades`
- CRUD completo

### Citas `/api/citas`
- CRUD + `PATCH /{id}/estado`
- Rutas jerárquicas: `/{id}/triaje`, `/{id}/atencion`, `/{id}/pagos`

Códigos HTTP: 200, 201, 400, 404, 409, 422, 500.

---

## 6. EVIDENCIA DE PRUEBAS FUNCIONALES (POSTMAN)

Importar: `postman/CitaSync_API_Tests.json`  
Variable de colección: `baseUrl` = `http://localhost:8080`

Antes de probar: `dotnet run --project src/ClinicaJesusDelNorte.API/ClinicaJesusDelNorte.API.csproj`

### Test Case 01 — Creación de Paciente (POST)

`POST http://localhost:8080/api/pacientes`

```json
{
  "tipoDocumento": "DNI",
  "numeroDocumento": "48129999",
  "nombres": "Max Ayrton",
  "apellidos": "Vásquez Ruiz",
  "tipoSeguro": "Particular",
  "activo": true
}
```

**Esperado:** HTTP **201 Created** con ID autogenerado y `codigoPaciente`.

**[CAPTURA TC01]** ← Pegar aquí captura de Postman (Status 201 + Body JSON).

### Test Case 02 — Consulta General de Citas (GET)

`GET http://localhost:8080/api/citas?page=0&size=20`

**Esperado:** HTTP **200 OK** con arreglo paginado (`data.items`).

**[CAPTURA TC02]** ← Pegar aquí captura de Postman (Status 200 + listado).

### Test Case 03 — Actualización de Estado (PATCH)

`PATCH http://localhost:8080/api/citas/1/estado`

```json
{
  "estado": "EnTriaje",
  "observacionCancelacion": null
}
```

**Esperado:** HTTP **200 OK**, estado `EnTriaje`.

**[CAPTURA TC03]** ← Pegar aquí captura de Postman (Status 200 + `estado: EnTriaje`).

### Resultado de revalidación automática (auditoría)

| Prueba | Código HTTP | Resultado |
|--------|-------------|-----------|
| Swagger `/swagger/v1/swagger.json` | 200 | OK |
| TC02 GET `/api/citas` | 200 | OK |
| TC01 POST `/api/pacientes` | 201 | OK |
| TC03 PATCH `/api/citas/1/estado` | 200 | OK |
| GET `/api/pacientes/99999` | 404 | OK |

---

## 7. INSTRUCCIONES DE EJECUCIÓN

```bash
git clone https://github.com/MaxVasquezR/clinica-jesus-del-norte.git
cd clinica-jesus-del-norte
dotnet restore
dotnet build
dotnet run --project src/ClinicaJesusDelNorte.API/ClinicaJesusDelNorte.API.csproj
```

- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`

Si el puerto 8080 está ocupado, la instancia previa ya está activa: usar Swagger o reiniciar el proceso.

---

## 8. CONCLUSIONES

Se entrega una API RESTful completa, tipada en C#, con arquitectura en capas, interfaces, DTOs, validaciones, manejo centralizado de errores y persistencia en memoria, alineada al nivel **Sobresaliente** de la rúbrica de evaluación parcial.
