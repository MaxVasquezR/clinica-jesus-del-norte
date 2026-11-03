using System.Reflection;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using ClinicaJesusDelNorte.API.Middleware;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Repository;
using ClinicaJesusDelNorte.API.Seguridad;
using ClinicaJesusDelNorte.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    {
        options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
            (valor, campo) => $"El valor '{valor}' no es válido para {campo}.");
        options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(
            valor => $"El valor '{valor}' no es válido.");
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = ValidationResponseFactory.Crear;
        options.SuppressMapClientErrors = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        options.AllowInputFormatterExceptionMessages = false;
    });

// Autenticación JWT: el usuario del token es el que queda en usuarioCreacion / usuarioActualizacion.
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
    ?? throw new InvalidOperationException("Falta la sección Jwt en appsettings.json.");
builder.Services.AddSingleton(jwt);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Emisor,
            ValidateAudience = true,
            ValidAudience = jwt.Audiencia,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = jwt.ClaveDeFirma(),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });
builder.Services.AddAuthorization(options =>
{
    // Todo endpoint exige sesión salvo los marcados con [AllowAnonymous] (login).
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Clínica Jesús del Norte — CitaSync API",
        Version = "v1",
        Description = "API RESTful para la gestión de citas médicas, pacientes, médicos y flujo clínico " +
                      "(triaje, atención y pagos). Persistencia simulada en memoria. " +
                      "Inicie sesión en POST /api/auth/login y use el botón Authorize con el token.",
        Contact = new OpenApiContact
        {
            Name = "Equipo CitaSync — Clínica Jesús del Norte",
            Email = "citasync@jesusdelnorte.pe"
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegue solo el token devuelto por POST /api/auth/login."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Seguridad
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IUsuarioActual, UsuarioActual>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

// Persistencia en memoria (singleton = datos vivos durante la ejecución)
builder.Services.AddSingleton(typeof(IRepositorio<>), typeof(Repositorio<>));
builder.Services.AddSingleton<Secuencias>();

// Servicios de negocio
builder.Services.AddScoped<IPersonaService, PersonaService>();
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IMedicoService, MedicoService>();
builder.Services.AddScoped<IEspecialidadService, EspecialidadService>();
builder.Services.AddScoped<IAseguradoraService, AseguradoraService>();
builder.Services.AddScoped<ICie10Service, Cie10Service>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<CitaFlujo>();
builder.Services.AddScoped<ICitaService, CitaService>();
builder.Services.AddScoped<IFlujoClinicoService, FlujoClinicoService>();
builder.Services.AddScoped<IPagoService, PagoService>();
builder.Services.AddScoped<DatosIniciales>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<DatosIniciales>().Cargar();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "CitaSync API v1");
    options.DocumentTitle = "Clínica Jesús del Norte — CitaSync";
    options.RoutePrefix = "swagger";
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
