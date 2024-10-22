using Api.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var misReglasCors = "ReglasCors";

// Configuraci�n de CORS
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
    config.RequireHttpsMetadata = false; // Habilitar la validaci�n de HTTPS en producci�n
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keybiter),
        ValidateIssuer = false, // Habilitar la validaci�n del emisor
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // Especificar el emisor v�lido
        ValidateAudience = false, // Habilitar la validaci�n de la audiencia
        ValidAudience = builder.Configuration["Jwt:Audience"], // Especificar la audiencia v�lida
    };
});

// Configuraci�n de Swagger con soporte de autenticaci�n JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });

    // Configuraci�n del esquema de seguridad JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Bienvenido al Encabezado de autorizaci�n JWT utilizando el esquema Bearer. \r\n\r\n 
                        Ingrese 'Bearer' [espacio] y luego su token en el ingreso de texto a continuaci�n.
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

// Configuraci�n de Swagger en el pipeline
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
