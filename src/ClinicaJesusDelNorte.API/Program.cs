using System.Reflection;
using System.Text.Json.Serialization;
using ClinicaJesusDelNorte.API.Middleware;
using ClinicaJesusDelNorte.API.Repository;
using ClinicaJesusDelNorte.API.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Dato inválido." : e.ErrorMessage)
                .ToList();

            var payload = ClinicaJesusDelNorte.API.Models.Common.ApiResponse<object>.Fail(
                "Error de validación de datos de entrada.",
                errors);

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(payload);
        };
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Clínica Jesús del Norte — CitaSync API",
        Version = "v1",
        Description = "API RESTful para la gestión de citas médicas, pacientes, médicos y flujo clínico " +
                      "(triaje, atención y pagos). Persistencia simulada en memoria.",
        Contact = new OpenApiContact
        {
            Name = "Equipo CitaSync — Clínica Jesús del Norte",
            Email = "citasync@jesusdelnorte.pe"
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Persistencia en memoria (singleton = datos vivos durante la ejecución)
builder.Services.AddSingleton<IInMemoryStore, InMemoryStore>();
builder.Services.AddSingleton<IPacienteRepository, PacienteRepository>();
builder.Services.AddSingleton<IMedicoRepository, MedicoRepository>();
builder.Services.AddSingleton<IEspecialidadRepository, EspecialidadRepository>();
builder.Services.AddSingleton<ICitaRepository, CitaRepository>();

// Servicios de negocio
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IMedicoService, MedicoService>();
builder.Services.AddScoped<IEspecialidadService, EspecialidadService>();
builder.Services.AddScoped<ICitaService, CitaService>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "CitaSync API v1");
    options.DocumentTitle = "Clínica Jesús del Norte — CitaSync";
    options.RoutePrefix = "swagger";
});

app.MapControllers();

app.Run();
