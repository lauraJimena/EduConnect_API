using EduConnect_API.Dtos;
using EduConnect_API.Repositories;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services;
using EduConnect_API.Services.Interfaces;
using EduConnect_API.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API_Software2 : " + builder.Configuration.GetValue<string>("Application:Environment"),
        Description = "API con implementación de JWT",
        Contact = new OpenApiContact
        {
            Name = "API Desarrollada por GrupoGestionElectricidad de la Universidad de Cundinamarca",
            Url = new Uri("https://www.ucundinamarca.edu.co")
        },
        License = new OpenApiLicense
        {
            Name = "Repositorio",
            Url = new Uri("https://github.com/AlanT218/backendConsumoE.git")
        }
    });

    //// Esta línea permite que Swagger lea los comentarios XML
    //var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
