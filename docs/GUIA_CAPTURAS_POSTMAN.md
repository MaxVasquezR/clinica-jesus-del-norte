# Guía mínima de capturas Postman (para el PDF)

Usá estas 3 capturas obligatorias en el informe técnico. Cada una debe mostrar claramente: **método**, **URL**, **Status** y **Body**.

## Preparación

1. Arrancar la API:
   ```bash
   dotnet run --project src/ClinicaJesusDelNorte.API/ClinicaJesusDelNorte.API.csproj
   ```
2. Abrir Postman → **Import** → seleccionar `postman/CitaSync_API_Tests.json`
3. Verificar variable `baseUrl` = `http://localhost:8080`
4. (Opcional) Abrir Swagger en el navegador: `http://localhost:8080/swagger` y tomar 1 captura extra del título **Clínica Jesús del Norte — CitaSync API**

## Captura TC01 — Crear paciente (POST)

1. Request: `01 - Pacientes` → `TC01 - Crear paciente (POST)`
2. Body (raw JSON):

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

3. Send → debe salir **201 Created**
4. Screenshot: ventana completa con Status verde `201` y `success: true` en el Body
5. Pegar en el informe en **[CAPTURA TC01]**

> Si el documento ya existe (409), cambiá `numeroDocumento` (ej. `48129998`) y volvé a probar.

## Captura TC02 — Listar citas (GET)

1. Request: `02 - Citas` → `TC02 - Consulta general de citas (GET)`
2. URL: `{{baseUrl}}/api/citas?page=0&size=20`
3. Send → **200 OK**
4. Screenshot mostrando `data.items` con al menos la cita seed `CJN-2026-0001`
5. Pegar en **[CAPTURA TC02]**

## Captura TC03 — Cambiar estado (PATCH)

1. Request: `02 - Citas` → `TC03 - Actualizar estado a EnTriaje (PATCH)`
2. Body:

```json
{
  "estado": "EnTriaje",
  "observacionCancelacion": null
}
```

3. Send → **200 OK** y `data.estado` = `"EnTriaje"`
4. Screenshot con Status 200 y el estado visible
5. Pegar en **[CAPTURA TC03]**

> Si la cita 1 ya no está en `Confirmada` (por pruebas previas), reiniciá la API (`Ctrl+C` y `dotnet run` de nuevo) para restaurar el seed.

## Qué debe verse en cada captura (checklist)

- [ ] URL completa visible (`localhost:8080/...`)
- [ ] Método HTTP (GET / POST / PATCH)
- [ ] Código de estado (200 / 201)
- [ ] Body JSON con `success` y `data`
- [ ] Nombre del proyecto o request visible en Postman

## Exportar a PDF

1. Abrí `docs/INFORME_TECNICO.md` en Word / Google Docs / VS Code + extensión Markdown PDF
2. Confirmá la URL del repo: https://github.com/MaxVasquezR/clinica-jesus-del-norte
3. Insertá las 3 capturas en los marcadores TC01–TC03 del Word (`docs/INFORME_TECNICO.docx`)
4. Exportá a PDF y subí **un solo integrante** a la plataforma virtual
