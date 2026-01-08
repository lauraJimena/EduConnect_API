using EduConnect_API.Dtos;
using EduConnect_API.Services;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TutoradoController : Controller
    {
        private readonly ITutoradoService _tutoradoService;
        private readonly IChatsService _chatsService;
      

        public TutoradoController(ITutoradoService tutoradoService, IChatsService chatsService)
        {
            _tutoradoService = tutoradoService;
            _chatsService = chatsService;
            
        }

        /// <summary>
        /// Obtiene el historial de tutorías de un tutorado con filtros opcionales
        /// </summary>

        [HttpGet("{idTutorado}/historial")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ObtenerHistorialTutorado(
            int idTutorado,
            [FromQuery] List<int>? idsEstado)
        {
            var datos = await _tutoradoService.ObtenerHistorialAsync(idTutorado, idsEstado);
            return Ok(datos);
        }
        /// <summary>
        /// Actualiza el perfil del tutorado
        /// </summary>
        [HttpPut("ActualizarPerfil")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ActualizarPerfil([FromBody] EditarPerfilDto perfil)
        {
            try
            {
                var result = await _tutoradoService.ActualizarPerfilTutorado(perfil);

                if (result > 0)
                    return Ok("Perfil actualizado con éxito");
                else
                    return NotFound("Usuario no encontrado o no se pudo actualizar");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
        /// <summary>
        /// Obtiene las solicitudes de tutorías de un tutorado con filtros opcionales
        /// </summary>
        [HttpPost("SolicitudesTutorias")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<SolicitudTutoriaDto>>> ObtenerSolicitudesTutorias([FromBody] FiltroSolicitudesDto filtro)
        {
            try
            {
                var solicitudes = await _tutoradoService.ObtenerSolicitudesTutorias(filtro);
                return Ok(solicitudes);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene los estados de las solicitudes de tutorías
        /// </summary>
        [HttpGet("EstadosSolicitud")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<EstadoSolicitudDto>>> ObtenerEstadosSolicitud()
        {
            try
            {
                var estados = await _tutoradoService.ObtenerEstadosSolicitud();
                return Ok(estados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
        /// <summary>
        /// Limpia los filtros de búsqueda
        /// </summary>
        private static string? Clean(string? s)
            => string.IsNullOrWhiteSpace(s) || s?.Trim().ToLower() == "string" ? null : s;

        /// <summary>
        /// Busca turores con filtros 
        /// </summary>
        [HttpPost("BuscarTutor")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Obtener([FromBody] BuscarTutorDto filtros)
        {
            filtros.Nombre = Clean(filtros.Nombre);
            filtros.MateriaNombre = Clean(filtros.MateriaNombre);
            filtros.Semestre = Clean(filtros.Semestre);
            filtros.CarreraNombre = Clean(filtros.CarreraNombre);
            if (filtros.IdEstado.HasValue && filtros.IdEstado.Value <= 0) filtros.IdEstado = null;

            var resultado = await _tutoradoService.ObtenerTutoresAsync(filtros);
            return Ok(resultado);
        }
        /// <summary>
        /// Crea una nueva solicitud de tutoría
        /// </summary>
        [HttpPost("CrearSolicitudTutoria")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CrearSolicitudTutoria([FromBody] SolicitudTutoriaRequestDto request)
        {
            try
            {
                var idTutoria = await _tutoradoService.CrearSolicitudTutoria(request);

                if (idTutoria > 0)
                {
                    // ✅ Crear el chat automáticamente vinculado a la tutoría
                    await _chatsService.CrearChat(new CrearChatDto
                    {
                        IdTutoria = idTutoria,
                        FechaCreacion = DateTime.Now
                    });

                    // ✅ Devolver respuesta exitosa
                    return Ok(new
                    {
                        mensaje = "Solicitud de tutoría creada con éxito.",
                        idTutoria
                    });
                }
                else
                {
                    return BadRequest(new { mensaje = "No se pudo crear la solicitud de tutoría." });
                }
            }
            catch (ArgumentException ex)
            {
                // ⚠️ Errores de validaciones lógicas o del trigger (RAISERROR)
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                // ⚙️ Solo agregar “Error interno” si el mensaje no viene del trigger
                string mensaje = ex.Message;
                if (!mensaje.StartsWith("❌"))
                    mensaje = "Error interno: " + mensaje;

                return StatusCode(500, new { mensaje });
            }
        }
        /// <summary>
        /// Crea un nuevo comentario para un tutor
        /// </summary>
        [HttpPost("CrearComentario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Comentario")]
        public async Task<IActionResult> CrearComentario(CrearComentarioDto comentario)
        {
            try
            {
                int idComentario = await _tutoradoService.CrearComentarioAsync(comentario);
                return Ok(new { idComentario, mensaje = "Comentario creado correctamente."});
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        /// <summary>
        /// Obtiene el ranking de tutores
        /// </summary>

        [HttpGet("RankingTutores")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<RankingTutorDto>>> ObtenerRankingTutores()
        {
            try
            {
                var ranking = await _tutoradoService.ObtenerRankingTutores();
                return Ok(ranking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
        /// <summary>
        /// Obtiene los comentarios de un tutor específico
        /// </summary>
        [HttpPost("ComentariosTutor")]
        public async Task<ActionResult<IEnumerable<ComentarioTutorInfoDto>>> ObtenerComentariosPorTutor([FromBody] ComentariosTutorRequestDto request)
        {
            try
            {
                var comentarios = await _tutoradoService.ObtenerComentariosPorTutor(request);
                return Ok(comentarios);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
        /// <summary>
        /// Obtiene el perfil de un tutor por su ID
        /// </summary>
        [HttpGet("PerfilTutor/{idTutor}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ObtenerPerfilTutor(int idTutor)
        {
            try
            {
                var tutor = await _tutoradoService.ObtenerPerfilTutorAsync(idTutor);
                if (tutor == null)
                    return NotFound("No se encontró el tutor especificado.");

                return Ok(tutor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }
        /// <summary>
        /// Obtiene la lista de todos los usuarios registrados en el sistema
        /// </summary>
        [HttpGet("ObtenerTutoradoPorId/{idUsuario}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ObtenerTutoradoPorId(int idUsuario)
        {
            try
            {
                // Llama al servicio
                var usuario = await _tutoradoService.ObtenerTutoradoPorIdAsync(idUsuario);

                // Si no existe
                if (usuario == null)
                    return NotFound(new { mensaje = "Tutorado no encontrado." });

                // Retorna los datos correctamente
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                // Manejo de errores controlado
                return StatusCode(500, new { mensaje = "Error interno del servidor: " + ex.Message });
            }
        }
        /// <summary>
        /// Envía un correo de confirmación de tutoría
        /// </summary>
        [HttpPost("EnviarConfirmacionTutoria")]
        public async Task<IActionResult> EnviarConfirmacionTutoria([FromQuery] int idTutoria)
        {
            try
            {
                var resultado = await _tutoradoService.EnviarCorreoConfirmacionTutoriaAsync(idTutoria);

                if (resultado)
                    return Ok("✅ Correo enviado correctamente.");
                else
                    return BadRequest("❌ No se pudo enviar el correo.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error en el servidor: {ex.Message}");
            }
        }
        /// <summary>
        /// Envía un correo de advertencia por calificación baja
        /// </summary>
        [HttpPost("EnviarCalificacionBaja")]
        public async Task<IActionResult> EnviarCalificacionBaja([FromQuery] int idComentario)
        {
            try
            {
                var resultado = await _tutoradoService.EnviarCorreoAdvertenciaCalificacionBajaAsync(idComentario);

                if (resultado)
                    return Ok("✅ Correo enviado correctamente.");
                else
                    return BadRequest("❌ No se pudo enviar el correo.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error en el servidor: {ex.Message}");
            }
        }



    }


}



    
   
    