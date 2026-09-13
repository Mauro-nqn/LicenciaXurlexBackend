//using LicenciaBackend.Data;
//using LicenciaBackend.Repositorios;
//using LicenciaBackend.Services;
//using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddScoped<ILicenciaRepositorio, LicenciaRepositorio>();

//// Configurar DB y servicios
//builder.Services.AddDbContext<LicenciaDbContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

//builder.Services.AddScoped<ILicenciaRepositorio, LicenciaRepositorio>();
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var config = new ConfigService();
//config.LeerConfiguracion();



//var app = builder.Build();

//// Configure the HTTP request pipeline.
////if (app.Environment.IsDevelopment())
////{
////    app.UseSwagger();
////    app.UseSwaggerUI();
////}

//var puerto = config.Puerto;
//app.Urls.Add($"http://0.0.0.0:{puerto}");


//app.UseRouting();

//app.UseSwagger();
//app.UseSwaggerUI();
//app.UseAuthentication();
//app.UseAuthorization();
//app.MapControllers();


////app.UseHttpsRedirection();



//app.Run();



using LicenciaBackend.Data;
using LicenciaBackend.Repositorios;
using LicenciaBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// --- Config personalizada (lee config.ini o config.azure.ini)
var cfg = new ConfigService();
cfg.LeerConfiguracion();

// --- Servicios
builder.Services.AddDbContext<LicenciaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))); // En Azure: App Settings > Connection strings
builder.Services.AddScoped<ILicenciaRepositorio, LicenciaRepositorio>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


//builder.Services.AddSwaggerGen();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LicenciaBackend",
        Version = "v1"
    });

    //  Definición del esquema de seguridad JWT
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Bearer {token}\"",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    //  Requisito global: agrega el esquema a todos los endpoints (podés ajustarlo)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            securityScheme,
            Array.Empty<string>()
        }
    });
});


// ==========  CONFIGURAR JWT AUTH ==========

// Lee la sección "Jwt" de appsettings/environment
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];      // clave simétrica
var jwtIssuer = jwtSection["Issuer"];   // p.ej. "LicenciaBackend"
var jwtAudience = jwtSection["Audience"]; // p.ej. "LicenciaTool"

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("Falta la configuración Jwt:Key en appsettings o variables de entorno.");
}

var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

// 1) Authentication con esquema JwtBearer
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;   // en dev podés poner false si usás solo HTTP
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = !string.IsNullOrWhiteSpace(jwtIssuer),
            ValidateAudience = !string.IsNullOrWhiteSpace(jwtAudience),
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

// 2) Authorization con una policy para administración de licencias
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("LicenciasAdmin", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("LicenciasAdmin");
    });
});



// --- Construir app
var app = builder.Build();




// === Detección de Azure ===
//bool isAzure = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));

// === Bind de URLs solo en LOCAL ===
// (en Azure, App Service administra el binding a 80/443)
//if (!isAzure)
//{
// Si querés HTTPS local, también podés usar cfg.UsaHttps y tu cert dev
//app.Urls.Add($"http://0.0.0.0:{cfg.Puerto}");
//}



// === Puerto / binding ===
var portEnv = Environment.GetEnvironmentVariable("PORT");

var puerto = int.TryParse(portEnv, out var railwayPort)
    ? railwayPort
    : cfg.Puerto;

app.Urls.Add($"http://0.0.0.0:{puerto}");

Console.WriteLine($"LicenciaBackend escuchando en puerto: {puerto}");


// === HTTPS redirection si el INI lo pide ===
if (cfg.UsaHttps)
{
    app.UseHttpsRedirection();
}

// === Swagger (mantenelo en Dev o cuando vos quieras) ===
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "iurixGo.API v1");
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); //  Inicia colapsado
    });
//}









// Si vas a usar Auth, registrala antes (por ahora no configuraste JwtBearer aquí)
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

