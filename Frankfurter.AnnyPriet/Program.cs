using FluentValidation;
using Frankfurter.AnnyPriet;
using Frankfurter.AnnyPriet.DTOS;
using Frankfurter.AnnyPriet.Endpoints;
using Frankfurter.AnnyPriet.Repositorios;
using Frankfurter.AnnyPriet.Servicios;
using Frankfurter.AnnyPriet.Utilidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Inicio de area de los servicios

builder.Services.AddCors(setupAction: opciones =>
{
    opciones.AddPolicy("Libre", configuracion =>
    {
        configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });

});

builder.Services.AddDbContext<AplicationDbContext>(opciones =>
opciones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<AplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<IdentityUser>>();

builder.Services.AddAuthentication().AddJwtBearer(opciones =>
opciones.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    IssuerSigningKeys = Llaves.ObtenerTodasLasLlaves(builder.Configuration),
    ClockSkew = TimeSpan.Zero
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOutputCache();

builder.Services.AddHttpClient();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<IValidator<CrearMonedaDTO>, CrearMonedaDTOValidator>();
builder.Services.AddScoped<IValidator<CrearTasaDeCambioDTO>, CrearTasaDeCambioDTOValidator>();
builder.Services.AddScoped<IValidator<CredencialesUsuarioDTO>, CredencialesUsuarioDTOValidator>();

builder.Services.AddScoped<IFrankfurterService, FrankfurterService>();
builder.Services.AddScoped<IRepositorioTasasDeCambios, RepositorioTasasDeCambios>();
builder.Services.AddScoped<IRepositorioMonedas, RepositorioMonedas>();

// Fin de area de los servicios

var app = builder.Build();

// inicio de los middleware

app.MapGroup("/api/frankfurter").MapFrankfurter();
app.MapGroup("/api/monedas").MapMoneda();
app.MapGroup("/api/tasas").MapTasaDeCambio();
app.MapGroup("/api/usuarios").MapUsuarios();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseOutputCache();

app.UseAuthentication();
app.UseAuthorization();

// Fin de area de los middleware

app.Run();
