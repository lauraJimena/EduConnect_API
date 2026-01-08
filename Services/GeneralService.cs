using Azure.Core;
using EduConnect_API.Dtos;
using EduConnect_API.Repositories;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;
using EduConnect_API.Utilities;
using MailKit.Security;
using MimeKit;
using Org.BouncyCastle.Crypto.Generators;
using System.Text.RegularExpressions;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace EduConnect_API.Services
{
    public class GeneralService : IGeneralService
    {
        private readonly IGeneralRepository _generalRepository;
        private readonly JwtSettingsDto _jwtSettings;
        private readonly BcryptHasherUtility _hasher;
        private readonly CorreoConfigUtility _config;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;
        private readonly CorreoManejoPlantillasUtility _plantillas;

        public GeneralService(IWebHostEnvironment env,
            CorreoManejoPlantillasUtility plantillas,
            IGeneralRepository generalRepository, 
            JwtSettingsDto jwtSettings, 
            BcryptHasherUtility hasher, 
            IConfiguration config, 
            ILogger<TutoradoService> logger)
        {
            _generalRepository = generalRepository;
            _jwtSettings = jwtSettings;
            _hasher = hasher;
            _logger = logger;
            _config = config.GetSection("CorreoConfigUtility").Get<CorreoConfigUtility>()!;
            _env = env;
            _plantillas = plantillas;
        }
        public async Task<int> RegistrarUsuario(CrearUsuarioDto usuario)
        {
            // Validaciones básicas obligatorias
            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                throw new Exception("El nombre del usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(usuario.Apellido))
                throw new Exception("El apellido del usuario es obligatorio.");

            if (usuario.IdTipoIdent <= 0)
                throw new Exception("Debe seleccionar un tipo de identificación válido.");

            if (string.IsNullOrWhiteSpace(usuario.NumIdent))
                throw new Exception("El número de identificación es obligatorio.");

            // Validación de correo
            if (string.IsNullOrWhiteSpace(usuario.Correo))
                throw new Exception("El correo es obligatorio.");
            else if (!Regex.IsMatch(usuario.Correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new Exception("El formato del correo no es válido.");

            // ❗ Validar que solo sea Gmail
            if (!usuario.Correo.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Solo se permiten correos de Gmail.");

            // Validación de número de identificación
            if (string.IsNullOrWhiteSpace(usuario.NumIdent))
                throw new Exception("El número de identificación es obligatorio.");
            else if (!Regex.IsMatch(usuario.NumIdent, @"^\d{7,10}$"))
                throw new Exception("El número de identificación que ingresaste no es válido.");

            // Validación de teléfono (10 dígitos numéricos)
            if (string.IsNullOrWhiteSpace(usuario.TelUsu))
                throw new Exception("El teléfono es obligatorio.");
            else if (!Regex.IsMatch(usuario.TelUsu, @"^[0-9]{10}$"))
                throw new Exception("El teléfono debe tener 10 dígitos.");

            // Validación de contraseña
            if (string.IsNullOrWhiteSpace(usuario.ContrasUsu))
                throw new Exception("La contraseña es obligatoria.");
            else if (!Regex.IsMatch(usuario.ContrasUsu, @"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{7,}$"))
                throw new Exception("La contraseña debe tener al menos 7 caracteres, incluir una mayúscula, un número y un carácter especial.");

            // Hashear la contraseña antes de guardarla
            usuario.ContrasUsu = _hasher.Hash(usuario.ContrasUsu);

            // Validaciones de llaves foráneas
            if (usuario.IdCarrera <= 0)
                throw new Exception("Debe seleccionar una carrera válida.");

            if (usuario.IdSemestre <= 0)
                throw new Exception("Debe seleccionar un semestre válido.");

            if (usuario.IdRol <= 0)
                throw new Exception("Debe asignar un rol válido.");

          
            if (await _generalRepository.ExisteNumeroIdentificacion(usuario.NumIdent))
                throw new Exception("Ya existe un usuario registrado con este número de identificación.");

          
            if (await _generalRepository.ExisteCorreo(usuario.Correo))
                throw new Exception("Ya existe un usuario registrado con este correo electrónico.");

            int idNuevoUsuario = await _generalRepository.RegistrarUsuario(usuario);

            if (idNuevoUsuario <= 0)
                throw new Exception("No se pudo registrar el usuario en la base de datos.");

            return idNuevoUsuario;
        }
        public async Task<bool> EnviarCorreoBienvenidaAsync(int idUsu)
        {
            try
            {
                // OBTENER LOS DATOS DEL USUARIO
                var datos = await _generalRepository.ObtenerDatosBienvenidaAsync(idUsu);
                if (datos == null)
                    return false;

                // CARGAR Y PERSONALIZAR LA PLANTILLA
                string plantilla = _plantillas.CargarPlantilla("Bienvenida.cshtml");
                string cuerpo = _plantillas.ReemplazarVariables(plantilla, new Dictionary<string, string>
        {
            { "Nombre", datos.Nombre },
            { "AñoActual", DateTime.Now.Year.ToString() }
        });

                // CREAR MENSAJE
                var mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress(_config.DisplayName, _config.Email));
                mensaje.To.Add(MailboxAddress.Parse(datos.Correo));
                mensaje.Subject = $"¡Bienvenido a EduConnect, {datos.Nombre}!";

                var builder = new BodyBuilder();

                // Insertar logo
                var rutaLogo = Path.Combine(_env.ContentRootPath, "Utilities", "PlantillasCorreo", "img", "Logo.png");
                var rutaSaludar = Path.Combine(_env.ContentRootPath, "Utilities", "PlantillasCorreo", "img", "Bienvenida.png");

               
                if (File.Exists(rutaLogo))
                {
                    var logo = builder.LinkedResources.Add(rutaLogo);
                    logo.ContentId = "logoEduConnect";
                }
                if (File.Exists(rutaSaludar))
                {
                    var logo = builder.LinkedResources.Add(rutaSaludar);
                    logo.ContentId = "saludar";
                }

                builder.HtmlBody = cuerpo;
                mensaje.Body = builder.ToMessageBody();

                // 5️⃣ ENVIAR CORREO
                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                await smtp.ConnectAsync(_config.Host, _config.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config.Email, _config.Password);
                await smtp.SendAsync(mensaje);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error al enviar correo de bienvenida: {ex.Message}");
                return false;
            }
        }

        public async Task<RespuestaInicioSesionDto> IniciarSesion(IniciarSesionDto usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.NumIdent))
                throw new Exception("El número de identificación es obligatorio.");
            else if (!Regex.IsMatch(usuario.NumIdent, @"^\d{7,10}$"))
                throw new Exception("El número de identificación que ingresaste no es válido."); 
            // Validación de contraseña
            if (string.IsNullOrWhiteSpace(usuario.ContrasUsu))
                throw new Exception("La contraseña es obligatoria.");

            // Ejecutar procedimiento de login en repositorio
            var result = await _generalRepository.IniciarSesion(usuario);

            if (result == null)
                throw new Exception("Número de identificación o contraseña incorrectos.");

            if (result.IdEstado != 1)
                throw new Exception("El usuario no está activo.");

            var ok = _hasher.Verify(usuario.ContrasUsu, result.ContrasenaHash);
            if (!ok)
                throw new Exception("Número de identificación o contraseña incorrectos.");

            // Crear DTO de respuesta
            RespuestaInicioSesionDto respuesta = new()
            {
                IdUsuario = result.IdUsu,
                IdRol = result.IdRol,
                DebeActualizarPassword = result.DebeActualizarPassword // 🔹 importante
            };

            // Generar token JWT
            respuesta = JwtUtility.GenTokenkey(respuesta, _jwtSettings);
            if (respuesta == null)
                throw new Exception("Error al generar el token de autenticación. Verifique la configuración JWT.");

            respuesta.Respuesta = 1;
            respuesta.Mensaje = "Inicio de sesión exitoso";

            return respuesta;
        }


        
        public async Task<List<CarreraDto>> ObtenerCarrerasAsync()
        {
            // No se necesita validación de parámetros porque no recibe ninguno,
            
            var carreras = await _generalRepository.ObtenerCarrerasAsync();

            if (carreras == null || !carreras.Any())
                throw new InvalidOperationException("No se encontraron carreras registradas.");

            return carreras;
        }
        public async Task<List<TipoIdentidadDto>> ObtenerTiposIdentidadAsync()
        {
            // No se necesita validación de parámetros porque no recibe ninguno,

            var tiposIdent = await _generalRepository.ObtenerTiposIdentidadAsync();

            if (tiposIdent == null || !tiposIdent.Any())
                throw new InvalidOperationException("No se encontraron carreras registradas.");

            return tiposIdent;
        }
        public async Task<int> ActualizarPasswordAsync(int idUsuario, string nuevaPassword)
        {
            if (string.IsNullOrWhiteSpace(nuevaPassword))
                throw new Exception("La contraseña es obligatoria.");
            else if (!Regex.IsMatch(nuevaPassword, @"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{7,}$"))
                throw new Exception("La contraseña debe tener al menos 7 caracteres, incluir una mayúscula, un número y un carácter especial.");

            // 🔹 Obtener la contraseña actual desde la BD
            var passwordActualHash = await _generalRepository.ObtenerPasswordActualAsync(idUsuario);

            if (passwordActualHash == null)
                throw new Exception("No se encontró el usuario especificado.");

            // 🔹 Verificar si la nueva contraseña es igual a la actual
            var esMisma = _hasher.Verify(nuevaPassword, passwordActualHash);
            if (esMisma)
                throw new Exception("La nueva contraseña no puede ser igual a la contraseña anterior.");

            // 🔹 Hashear y guardar
            var nuevaPasswordHash = _hasher.Hash(nuevaPassword);

            return await _generalRepository.ActualizarPassword(idUsuario, nuevaPasswordHash);
        }
        public async Task<bool> EnviarCorreoRecuperacionAsync(string correo)
        {
            try
            {
                // 1️⃣ Validar que el correo exista
                var usuario = await _generalRepository.ObtenerUsuarioPorCorreoAsync(correo);
                if (usuario == null)
                    return false;

                // 2️⃣ Generar token y guardar
                string token = Guid.NewGuid().ToString();
                await _generalRepository.GuardarTokenRecuperacionAsync(correo, token);

                
                string enlace = $"https://localhost:83/General/RestablecerContrasena?token={token}";

               
               
                string plantilla = _plantillas.CargarPlantilla("RecuperarPassword.cshtml");
                string cuerpo = _plantillas.ReemplazarVariables(plantilla, new Dictionary<string, string>
        {
            { "NombreUsuario", usuario.Nombre },
            { "Enlace", enlace },
            { "AñoActual", DateTime.Now.Year.ToString() }
        });

                // 5️⃣ Crear mensaje y enviar con MailKit
                var mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress(_config.DisplayName, _config.Email));
                mensaje.To.Add(MailboxAddress.Parse(correo));
                mensaje.Subject = "🔒 Recuperación de contraseña - EduConnect";

                var builder = new BodyBuilder();
                var rutaLogo = Path.Combine(_env.ContentRootPath, "Utilities", "PlantillasCorreo", "img", "Logo.png");
                if (File.Exists(rutaLogo))
                {
                    var logo = builder.LinkedResources.Add(rutaLogo);
                    logo.ContentId = "logoEduConnect";
                }

                builder.HtmlBody = cuerpo;
                mensaje.Body = builder.ToMessageBody();

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                await smtp.ConnectAsync(_config.Host, _config.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config.Email, _config.Password);
                await smtp.SendAsync(mensaje);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al enviar correo de recuperación: {ex.Message}");
                return false;
            }
        }
        public async Task<(bool Ok, string Msg)> RestablecerContrasenaAsync(RestablecerContrasenaDto dto)
        {
            if (dto == null)
                return (false, "Los datos enviados son inválidos.");

            if (string.IsNullOrWhiteSpace(dto.Token))
                return (false, "El token de recuperación es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.NuevaPassword))
                return (false, "La nueva contraseña es obligatoria.");

            // 🔹 Validar seguridad mínima
            if (dto.NuevaPassword.Length < 7)
                return (false, "La contraseña debe tener al menos 7 caracteres.");

            // 🔹 Obtener info del token
            var tokenInfo = await _generalRepository.ObtenerTokenRecuperacionAsync(dto.Token);
            if (tokenInfo == null)
                return (false, "El enlace de recuperación no existe o ha expirado.");

            if (tokenInfo.Usado)
                return (false, "El enlace de recuperación ya fue utilizado.");

            if (tokenInfo.FechaExpira < DateTime.Now)
                return (false, "El enlace de recuperación ha expirado.");

            // 🔹 Hashear nueva contraseña
            var hasher = new BcryptHasherUtility();
            string nuevaHash = hasher.Hash(dto.NuevaPassword);

            // 🔹 Actualizar contraseña
            await _generalRepository.ActualizarPasswordPorCorreo(tokenInfo.Correo, nuevaHash);

            // 🔹 Marcar token como usado
            await _generalRepository.MarcarTokenUsado(dto.Token);

            return (true, "✅ Contraseña restablecida correctamente. Ya puedes iniciar sesión.");
        }



    }

}
