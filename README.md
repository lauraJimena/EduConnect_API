<p align="center">
  <img width="970" height="270" alt="Logo" src="https://github.com/user-attachments/assets/5c0861de-90c4-4e83-9aec-47a9f76a6961" />
</p>

<h1 align="center"><b>EDUCONNECT API</b></h1>

<p align="center">
  <i>Backend del sistema de tutorías académicas desarrollado en .NET 8 / C#</i>
</p>

---

**EduConnect_API** corresponde al backend del sistema de tutorías académicas **EduConnect**, desarrollado en **.NET 8 / C#** y respaldado por **SQL Server**.  
El servicio expone una **API REST** para la gestión de usuarios (tutores, tutorados, coordinadores y administradores), materias, sesiones de tutoría, calificaciones y mensajería, con **autenticación JWT** y **CORS** configurado para el frontend oficial.

<h2><b>Objetivos del proyecto</b></h2>

- Proveer servicios REST confiables para registrar y consultar la información académica del programa de tutorías.  
- Centralizar la autenticación/autorización mediante **JWT**.  
- Integrar datos con **procedimientos almacenados** para reportes y operaciones clave, además de integrar **triggers**.  
- Asegurar la comunicación exclusiva con el frontend autorizado mediante **CORS**.  

<h2><b>Principales componentes de la arquitectura</b></h2>

- Controllers: exponen endpoints y orquestan servicios.
- Services: encapsulan reglas de negocio.
- Repositories: acceso a datos y ejecución de SP.
- Utilities: utilidades transversales (por ejemplo, BcryptHasherUtility, configuración JWT).
  
<h2><b>Tecnologías</b></h2>

- ASP.NET Core Web API (.NET 8).
- SQL Server.
- Entity Framework Core.
- JWT Bearer Authentication.
- Swagger.
- CORS.
- Inyección de dependencias (DI).

<h2><b>Requisitos previos</b></h2>

- .NET SDK 8.0
- [Visual Studio 2022] o [VS Code]
- [SQL Server] y [SQL Server Management Studio (SSMS)]
- Permisos para crear base de datos y ejecutar scripts

<h2><b>Base de datos</b></h2>
<h3>Creación de base</h3>

```sql
CREATE DATABASE EduConnect;
GO
```
<h3>Ejecución de objetos</h3>

Se deben crear las tablas, vistas y procedimientos almacenados necesarios para el funcionamiento del sistema.
Entre los procedimientos almacenados utilizados por la solución se encuentran:

- sp_ObtenerRankingTutores: Retorna los tres tutores activos con mejor promedio, incluyendo carrera, semestre, avatar y materias.
- usp_Reporte_TutoradosActivos: Genera un reporte de tutorados con total de tutorías, última fecha y materias solicitadas.
- usp_Tutores_ListarMaterias: Lista tutores con filtros opcionales (nombre, materia, semestre, carrera, estado) y paginación (@Skip, @Take).
- usp_Tutoria_ObtenerDatosCorreo: Obtiene tutor, tutorado, materia, fecha y hora para construir mensajes de correo.

<i>Nota:</i> La solución asume nomenclaturas y relaciones coherentes con estos procedimientos almacenados (SP).
<h2><b>Configuración de seguridad</b></h2> <h3>JSON Web Token (JWT)</h3>

En el archivo <b>appsettings.json</b> se definen las claves y validaciones utilizadas para la autenticación y autorización mediante JWT:
```json
"JsonWebTokenKeys": {
  "IssuerSigningKey": "CLAVE_SECRETA_SEGURA",
  "ValidIssuer": "EduConnectAPI",
  "ValidAudience": "EduConnectUsers",
  "ValidateIssuer": true,
  "ValidateAudience": true,
  "RequireExpirationTime": true,
  "ValidateLifetime": true
}
```
<h3>CORS (Cross-Origin Resource Sharing)</h3>

Se restringe el consumo al frontend oficial de EduConnect:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("https://localhost:7270")   // URL del frontend .NET
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

app.UseCors("AllowFrontend");
```
<h3>Resultados esperados de las pruebas:</h3>

- Solicitudes desde https://localhost:7270 → permitidas
- Solicitudes desde otros orígenes/puertos → bloqueadas (CORS/CSP)
- Solicitudes a endpoints protegidos sin token → 401 Unauthorized

<h2><b>Configuración de la conexión</b></h2>
En el archivo <b>appsettings.json</b> se debe configurar la cadena de conexión a la base de datos SQL Server:

```csharp
"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR;Database=EduConnect;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```
<h2><b>Instalación y ejecución</b></h2> 
<h3>Clonado</h3>

```bash
git clone https://github.com/lauraJimena/EduConnect_API.git
cd EduConnect_API
```
<h3>Restauración, compilación y ejecución (terminal)</h3>

```bash
dotnet restore
dotnet build
dotnet run
```
<h3>Ejecución en Visual Studio</h3>

Seleccionar el perfil <b>EduConnect_API</b> y ejecutar con <b>F5</b> o <b>CTRL + F5</b>.

<h3>Swagger</h3>

La documentación y pruebas interactivas de la API se encuentran en:
https://localhost:7003/swagger

<h2><b>Buenas prácticas aplicadas</b></h2>

- Validación estricta de JWT (issuer, audience, firma, expiración).
- HTTPS habilitado (UseHttpsRedirection).
- CORS restringido al dominio autorizado.
- Separación de capas (Controllers → Services → Repositories).
- Documentación completa en Swagger con esquema de seguridad Bearer.

<h2><b>Autoría</b></h2>

- Laura Jimena Herreño Rubiano
- Andrés Mateo Morales Gonzalez
- Juan Sebastián Moreno
- Edwin Felipe Garavito Izquierdo
<br>Estudiantes de Ingeniería de Sistemas – Universidad de Cundinamarca.
<br>Correo: 📧<a href="mailto:notificaciones.educonnect@gmail.com">notificaciones.educonnect@gmail.com
</a>

<h2><b>Licencia</b></h2>

Proyecto académico con fines educativos.
Su reutilización requiere la referencia a los autores y al proyecto EduConnect.
