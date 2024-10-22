using Api.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var misReglasCors = "ReglasCors";

// Configuración de CORS
builder.Services.AddCors(option =>
    option.AddPolicy(name: misReglasCors,
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        })
);

builder.Services.AddRepositories();

// Add services to the container.
builder.Configuration.AddJsonFile("appsettings.json");
var secret = builder.Configuration.GetSection("settings").GetSection("secretaKey").ToString();
var keybiter = Encoding.UTF8.GetBytes(secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false; // Habilitar la validación de HTTPS en producción
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keybiter),
        ValidateIssuer = false, // Habilitar la validación del emisor
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // Especificar el emisor válido
        ValidateAudience = false, // Habilitar la validación de la audiencia
        ValidAudience = builder.Configuration["Jwt:Audience"], // Especificar la audiencia válida
    };
});

// Configuración de Swagger con soporte de autenticación JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });

    // Configuración del esquema de seguridad JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Bienvenido al Encabezado de autorización JWT utilizando el esquema Bearer. \r\n\r\n 
                        Ingrese 'Bearer' [espacio] y luego su token en el ingreso de texto a continuación.
                        \r\n\r\nEjemplo: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddControllers();
var app = builder.Build();

// Configuración de Swagger en el pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api - v1");
});

app.UseExceptionHandler("/Error");
app.UseCors(misReglasCors);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
