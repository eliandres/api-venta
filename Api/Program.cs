using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
var misReglasCors = "ReglasCors";

builder.Services.AddCors(option =>
option.AddPolicy(name: misReglasCors,
builder =>
{
    builder.AllowAnyOrigin().AllowAnyOrigin().AllowAnyMethod();
})
); ;

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


// Agregar la configuraci�n de Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiVenta", Version = "v1" });
});


builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api.010");
});

app.UseExceptionHandler("/Error");
app.UseCors(misReglasCors);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
