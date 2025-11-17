using EduConnect_API.Dtos;
using EduConnect_API.Repositories;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services;
using EduConnect_API.Services.Interfaces;
using EduConnect_API.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

System.Net.ServicePointManager.SecurityProtocol =
    System.Net.SecurityProtocolType.Tls12;


var builder = WebApplication.CreateBuilder(args);
// Configuración de JwtSettings
var bindJwtSettings = new JwtSettingsDto();
builder.Configuration.Bind("JsonWebTokenKeys", bindJwtSettings);
builder.Services.AddSingleton(bindJwtSettings);
builder.Services.AddSingleton<BcryptHasherUtility>();

builder.Services.AddAuthentication(options =>
{
options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = bindJwtSettings.ValidateIssuerSigningKey,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(bindJwtSettings.IssuerSigningKey)),
        ValidateIssuer = bindJwtSettings.ValidateIssuer,
        ValidIssuer = bindJwtSettings.ValidIssuer,
        ValidateAudience = bindJwtSettings.ValidateAudience,
        ValidAudience = bindJwtSettings.ValidAudience,
        RequireExpirationTime = bindJwtSettings.RequireExpirationTime,
        ValidateLifetime = bindJwtSettings.ValidateLifetime,
        ClockSkew = TimeSpan.Zero
    };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Append("Token-Expired-Time", "true");
                }
                return Task.CompletedTask;
            }
        };
});


// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IGeneralService, GeneralService>();
builder.Services.AddScoped<IGeneralRepository, GeneralRepository>();
builder.Services.AddScoped<IAdministradorRepository, AdministradorRepository>();
builder.Services.AddScoped<IAdministradorService, AdministradorService>();
builder.Services.AddScoped<ICoordinadorRepository, CoordinadorRepository>();
builder.Services.AddScoped<ICoordinadorService, CoordinadorService>();
builder.Services.AddScoped<ITutoradoRepository, TutoradoRepository>();
builder.Services.AddScoped<ITutoradoService, TutoradoService>();
builder.Services.AddScoped<ITutorRepository, TutorRepository>();
builder.Services.AddScoped<ITutorService, TutorService>();
builder.Services.AddScoped<IChatsService, ChatsService>();
builder.Services.AddScoped<IChatsRepository, ChatsRepository>();
builder.Services.AddScoped<DbContextUtility>();
builder.Services.AddSingleton<CorreoManejoPlantillasUtility>();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API - Sistema de Tutorías Académicas UdeC - EDUCONNECT",
        Description = "Esta API forma parte del sistema de tutorías académicas entre estudiantes de la Universidad de Cundinamarca. "
                + "Proporciona servicios REST para la gestión de usuarios, tutorías, autenticación JWT y comunicación entre tutores y tutorados. "
                + "\n\n**Características principales:**\n"
                + "- Autenticación y autorización mediante JWT.\n"
                + "- Gestión de usuarios (tutores, tutorados, coordinadores, administradores).\n"
                + "- Registro y consulta de tutorías.\n"
                + "- Chat interno y seguimiento del proceso académico.\n"
                + "- Integración con base de datos SQL Server.\n\n",               
        Contact = new OpenApiContact
        {
            Name = "Desarrollado por estudiantes de Ingeniería de Sistemas - Universidad de Cundinamarca",
            Url = new Uri("https://www.ucundinamarca.edu.co")
          
        },
        License = new OpenApiLicense
        {
            Name = "Repositorio del proyecto (GitHub)",
            Url = new Uri("https://github.com/lauraJimena/EduConnect_API")
        }
    });


    //// Esta línea permite que Swagger lea los comentarios XML
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese su token JWT válido en el campo de autorización.\r\nEjemplo: \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\".",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CONFIGURACIÓN DE CORS SOLO PARA EL FRONT .NET

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:83")  // URL del front .NET Core
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


var app = builder.Build();

// Habilitar Swagger SIEMPRE (también en IIS)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EduConnect API v1");
    c.RoutePrefix = "swagger"; // obligatorio para que abra correctamente
});


app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthentication();


app.UseAuthorization();

app.MapControllers();

app.Run();
