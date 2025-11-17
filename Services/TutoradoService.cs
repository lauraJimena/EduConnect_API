using EduConnect_API.Dtos;
using EduConnect_API.Dtos;
using EduConnect_API.Repositories;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;
using EduConnect_API.Services.Interfaces;
using EduConnect_API.Utilities;
using MailKit.Security;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Net.Mail;
using System.Text.RegularExpressions;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace EduConnect_API.Services
{
    public class TutoradoService : ITutoradoService
    {
        private readonly ITutoradoRepository _tutoradoRepository;
        private readonly CorreoConfigUtility _config;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;
        private readonly CorreoManejoPlantillasUtility _plantillas;

        public TutoradoService(IWebHostEnvironment env,
            CorreoManejoPlantillasUtility plantillas, ITutoradoRepository tutoradoRepository, IConfiguration config, ILogger<TutoradoService> logger)
        {
            _tutoradoRepository = tutoradoRepository;
            _logger = logger;
            _config = config.GetSection("CorreoConfigUtility").Get<CorreoConfigUtility>()!;
            _env = env;
            _plantillas = plantillas;
        }

        public Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialAsync(int idTutorado, List<int>? idsEstado)
            => _tutoradoRepository.ObtenerHistorialTutoradoAsync(idTutorado, idsEstado);
        public async Task<IEnumerable<ObtenerTutorDto>> ObtenerTutoresAsync(BuscarTutorDto filtros)
        {
            if (filtros == null)
                throw new ArgumentNullException(nameof(filtros));

            // --- Validación de paginación ---
            if (filtros.Page <= 0)
                filtros.Page = 1;

            if (filtros.PageSize <= 0)
                filtros.PageSize = 20;
            else if (filtros.PageSize > 100)
                filtros.PageSize = 100;

            // --- Validaciones básicas de texto ---
            if (!string.IsNullOrWhiteSpace(filtros.Nombre) && filtros.Nombre.Length < 2)
                throw new ArgumentException("El nombre debe tener al menos 2 caracteres para buscar.");

            if (!string.IsNullOrWhiteSpace(filtros.MateriaNombre) && filtros.MateriaNombre.Length < 2)
                throw new ArgumentException("La materia debe tener al menos 2 caracteres para buscar.");

            if (!string.IsNullOrWhiteSpace(filtros.CarreraNombre) && filtros.CarreraNombre.Length < 2)
                throw new ArgumentException("La carrera debe tener al menos 2 caracteres para buscar.");

            // --- Validaciones numéricas --

            if (filtros.IdEstado.HasValue && filtros.IdEstado < 0)
                throw new ArgumentException("El estado es inválido.");

            // --- Llamada al repositorio ---
            return await _tutoradoRepository.ObtenerTutoresAsync(filtros);
        }

        public async Task<int> ActualizarPerfilTutorado(EditarPerfilDto tutorado)
        {
            // Validación de ID
            if (tutorado.IdUsu <= 0)
                throw new ArgumentException("El IdUsuario es obligatorio para actualizar.");

            // Verificar que el usuario existe
            if (!await _tutoradoRepository.ExisteUsuario(tutorado.IdUsu))
                throw new ArgumentException("El tutorado no existe.");

            // Validar que el usuario tiene rol de Tutorado (rol 1)
            int rolUsuario = await _tutoradoRepository.ObtenerRolUsuario(tutorado.IdUsu);
            if (rolUsuario != 1)
                throw new ArgumentException("El usuario no tiene permisos de tutorado para editar este perfil.");

            // Resto de validaciones...
            if (string.IsNullOrWhiteSpace(tutorado.Nombre))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(tutorado.Apellido))
                throw new ArgumentException("El apellido es obligatorio.");

            if (tutorado.IdTipoIdent <= 0)
                throw new ArgumentException("El tipo de identificación es obligatorio.");

            if (string.IsNullOrWhiteSpace(tutorado.NumIdent))
                throw new ArgumentException("El número de identificación es obligatorio.");
            else if (!Regex.IsMatch(tutorado.NumIdent, @"^\d{7,10}$"))
                throw new ArgumentException("El número de identificación debe tener entre 7 y 10 dígitos.");

            if (string.IsNullOrWhiteSpace(tutorado.TelUsu))
                throw new ArgumentException("El teléfono es obligatorio.");
            else if (!Regex.IsMatch(tutorado.TelUsu, @"^3\d{9}$"))
                throw new ArgumentException("El teléfono debe tener 10 dígitos y empezar en 3.");

            if (string.IsNullOrWhiteSpace(tutorado.Correo))
                throw new ArgumentException("El correo es obligatorio.");
            else if (!Regex.IsMatch(tutorado.Correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("El correo no tiene un formato válido.");

            return await _tutoradoRepository.ActualizarPerfil(tutorado);
        }
        public async Task<IEnumerable<SolicitudTutoriaDto>> ObtenerSolicitudesTutorias(FiltroSolicitudesDto filtro)
        {
            // Validaciones
            if (filtro.IdTutorado <= 0)
                throw new ArgumentException("El ID del tutorado es obligatorio.");

            if (!await _tutoradoRepository.ExisteUsuario(filtro.IdTutorado))
                throw new ArgumentException("El tutorado no existe.");

            // Validar que el usuario tiene rol de Tutorado (rol 1)
            int rolUsuario = await _tutoradoRepository.ObtenerRolUsuario(filtro.IdTutorado);
            if (rolUsuario != 1)
                throw new ArgumentException("El usuario no tiene permisos de tutorado.");

            return await _tutoradoRepository.ObtenerSolicitudesTutorias(filtro);
        }

        public async Task<IEnumerable<EstadoSolicitudDto>> ObtenerEstadosSolicitud()
        {
            return await _tutoradoRepository.ObtenerEstadosSolicitud();
        }
        public async Task<int> CrearSolicitudTutoria(SolicitudTutoriaRequestDto solicitud)
        {
            try
            {
                // ⚙️ Validaciones básicas (las de tu código original)
                if (solicitud.IdTutorado <= 0)
                    throw new ArgumentException("El ID del tutorado es obligatorio.");

                if (solicitud.IdTutor <= 0)
                    throw new ArgumentException("El ID del tutor es obligatorio.");

                if (solicitud.IdMateria <= 0)
                    throw new ArgumentException("El ID de la materia es obligatorio.");

                if (solicitud.IdModalidad <= 0)
                    throw new ArgumentException("El ID de la modalidad es obligatorio.");

                // Validar que el tutorado existe y es tutorado
                if (!await _tutoradoRepository.ExisteUsuario(solicitud.IdTutorado))
                    throw new ArgumentException("El tutorado no existe.");

                int rolUsuario = await _tutoradoRepository.ObtenerRolUsuario(solicitud.IdTutorado);
                if (rolUsuario != 1)
                    throw new ArgumentException("El usuario no tiene permisos de tutorado.");

                // Validar que el tutor existe y es tutor
                if (!await _tutoradoRepository.ExisteTutor(solicitud.IdTutor))
                    throw new ArgumentException("El tutor no existe o no tiene permisos de tutor.");

                if (!await _tutoradoRepository.ExisteModalidad(solicitud.IdModalidad))
                    throw new ArgumentException("La modalidad no existe.");

                // Validar fecha y hora
                if (solicitud.Fecha < DateTime.Today)
                    throw new ArgumentException("La fecha no puede ser en el pasado.");

                if (string.IsNullOrWhiteSpace(solicitud.Hora))
                    throw new ArgumentException("La hora es obligatoria.");

                if (!TimeSpan.TryParse(solicitud.Hora, out TimeSpan hora))
                    throw new ArgumentException("El formato de la hora no es válido. Use formato HH:mm (ej: 14:30).");

                // Validar tema
                if (string.IsNullOrWhiteSpace(solicitud.Tema))
                    throw new ArgumentException("El tema es obligatorio.");

                if (solicitud.Tema.Length < 5)
                    throw new ArgumentException("El tema debe tener al menos 5 caracteres.");

                // Validar comentario adicional (si existe)
                if (!string.IsNullOrWhiteSpace(solicitud.ComentarioAdicional) && solicitud.ComentarioAdicional.Length > 500)
                    throw new ArgumentException("El comentario adicional no puede exceder los 500 caracteres.");

                // ✅ Llamada al repository
                return await _tutoradoRepository.CrearSolicitudTutoria(solicitud);
            }
            catch (SqlException ex)
            {
                // Captura el mensaje del trigger limpio
                string mensaje = ex.Message.Split("The transaction ended")[0].Trim();
                throw new ArgumentException(mensaje);
            }
            catch (Exception ex)
            {
                // 💡 Si el mensaje viene del trigger (empieza con ❌), no agregues prefijo
                string mensaje = ex.Message;
                if (mensaje.StartsWith("❌"))
                    throw new ArgumentException(mensaje);

                throw new Exception("Error al crear la solicitud de tutoría: " + mensaje);
            }
        }



        public async Task<int> CrearComentarioAsync(CrearComentarioDto comentario)
        {
            
            if (comentario == null)
                throw new ArgumentException("Los datos del comentario son obligatorios.");

            if (comentario.Calificacion < 1 || comentario.Calificacion > 5)
                throw new ArgumentException("La calificación debe estar entre 1 y 5 estrellas.");

            if (comentario.IdTutor <= 0 || comentario.IdTutorado <= 0)
                throw new ArgumentException("Faltan los datos del tutor o tutorado.");

            // Insertar en la base de datos y devolver el ID generado
            int idComentario = await _tutoradoRepository.CrearComentarioAsync(comentario);

            if (idComentario <= 0)
                throw new Exception("No se pudo crear el comentario correctamente.");

            return idComentario;
        }


        public async Task<IEnumerable<RankingTutorDto>> ObtenerRankingTutores()
        {
            return await _tutoradoRepository.ObtenerRankingTutores();
        }

        public async Task<IEnumerable<ComentarioTutorInfoDto>> ObtenerComentariosPorTutor(ComentariosTutorRequestDto request)
        {
            // Validaciones
            if (request.IdTutor <= 0)
                throw new ArgumentException("El ID del tutor es obligatorio.");

            // Validar que el tutor existe
            if (!await _tutoradoRepository.ExisteTutor(request.IdTutor))
                throw new ArgumentException("El tutor no existe.");

            return await _tutoradoRepository.ObtenerComentariosPorTutor(request.IdTutor);
        }

        public async Task<PerfilTutorDto> ObtenerPerfilTutorAsync(int idTutor)
        {
            try
            {
                return await _tutoradoRepository.ObtenerPerfilTutorAsync(idTutor);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el perfil del tutor: " + ex.Message);
            }
        }
        public async Task<ObtenerUsuarioDto> ObtenerTutoradoPorIdAsync(int idTutorado)
        {
            try
            {
                return await _tutoradoRepository.ObtenerTutoradoPorId(idTutorado);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el perfil del tutor: " + ex.Message);
            }
        }
        public async Task<bool> EnviarCorreoConfirmacionTutoriaAsync(int idTutoria)
        {
            try
            {
                var datos = await _tutoradoRepository.ObtenerDatosTutoriaAsync(idTutoria);
                if (datos == null)
                    return false;

                
                string plantilla = _plantillas.CargarPlantilla("ConfirmacionTutoria.cshtml");


                string cuerpo = _plantillas.ReemplazarVariables(plantilla, new Dictionary<string, string>
        {
            { "NombreTutorado", datos.NombreTutorado },
            { "NombreTutor", datos.NombreTutor },
            { "Materia", datos.Materia },
            { "Fecha", datos.Fecha.ToString("dd/MM/yyyy") },
            { "Hora", datos.Hora },
            { "AñoActual", DateTime.Now.Year.ToString() }
        });

        
                var mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress(_config.DisplayName, _config.Email));
                mensaje.To.Add(MailboxAddress.Parse(datos.CorreoTutorado));
                mensaje.Subject = $"Solicitud de Tutoría - {datos.Materia}";

                
                var builder = new BodyBuilder();

                //Agregar imagen embebida
               
                var rutaLogo = Path.Combine(_env.ContentRootPath, "Utilities", "PlantillasCorreo", "img", "Logo.png");
                if (File.Exists(rutaLogo))
                {
                    var logo = builder.LinkedResources.Add(rutaLogo);
                    logo.ContentId = "logoEduConnect"; // debe coincidir con el cid: del HTML
                }
               
                var rutaSaludar = Path.Combine(_env.ContentRootPath, "Utilities", "PlantillasCorreo", "img", "Saludar.png");
                if (File.Exists(rutaSaludar))
                {
                    var saludarImg = builder.LinkedResources.Add(rutaSaludar);
                    saludarImg.ContentId = "saludar"; // debe coincidir con el cid del HTML
                }

                builder.HtmlBody = cuerpo;
                mensaje.Body = builder.ToMessageBody();

                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                _logger.LogInformation($"📡 Iniciando conexión SMTP a {_config.Host}:{_config.Port}...");

                await smtp.ConnectAsync(_config.Host, _config.Port, SecureSocketOptions.StartTls);
                _logger.LogInformation("✅ Conexión SMTP establecida correctamente.");

                await smtp.AuthenticateAsync(_config.Email, _config.Password);
                _logger.LogInformation("🔐 Autenticación exitosa.");

                await smtp.SendAsync(mensaje);
                _logger.LogInformation($"📨 Correo enviado correctamente a {datos.CorreoTutorado}.");

                await smtp.DisconnectAsync(true);
                _logger.LogInformation("🔌 Desconectado de SMTP.");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al enviar correo: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EnviarCorreoAdvertenciaCalificacionBajaAsync(int idComentario)
        {
            try
            {
                // 1️⃣ Obtener los datos del comentario (tutor, tutorado, calificación, texto)
                var datos = await _tutoradoRepository.ObtenerDatosComentarioAsync(idComentario);
                if (datos == null)
                    return false;

                if (datos.Calificacion > 2)
                    return false; // solo enviamos si la calificación es 2 o menor

               
                
                string plantilla = _plantillas.CargarPlantilla("AdvertenciaCalificacionBaja.cshtml");
                string cuerpo = _plantillas.ReemplazarVariables(plantilla, new Dictionary<string, string>
        {
            { "NombreTutor", datos.NombreTutor },
            { "NombreTutorado", datos.NombreTutorado },
            { "Calificacion", datos.Calificacion.ToString() },
            { "Comentario", datos.Texto },
            { "AñoActual", DateTime.Now.Year.ToString() }
        });

                // 3️⃣ Crear mensaje MIME
                var mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress(_config.DisplayName, _config.Email));
                mensaje.To.Add(MailboxAddress.Parse(datos.CorreoTutor));
                mensaje.Subject = "⚠️ Alerta: Calificación baja recibida de un tutorado";

                var builder = new BodyBuilder();

               
                var rutaLogo = Path.Combine(_env.ContentRootPath, "Utilities", "PlantillasCorreo", "img", "Logo.png");
               
                if (File.Exists(rutaLogo))
                {
                    var logo = builder.LinkedResources.Add(rutaLogo);
                    logo.ContentId = "logoEduConnect";
                }
                // Ícono de alerta
                var rutaIcono = Path.Combine(_env.ContentRootPath, "Utilities", "PlantillasCorreo", "img", "alerta.png");
                if (File.Exists(rutaIcono))
                {
                    var icono = builder.LinkedResources.Add(rutaIcono);
                    icono.ContentId = "iconoAlerta";
                }

                builder.HtmlBody = cuerpo;
                mensaje.Body = builder.ToMessageBody();

                // 5️⃣ Enviar correo
                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                _logger.LogInformation($"📡 Conectando al servidor SMTP {_config.Host}:{_config.Port}...");

                await smtp.ConnectAsync(_config.Host, _config.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config.Email, _config.Password);
                await smtp.SendAsync(mensaje);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"📨 Correo de advertencia enviado correctamente a {datos.CorreoTutor}.");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al enviar correo de advertencia: {ex.Message}");
                return false;
            }
        }




    }
}