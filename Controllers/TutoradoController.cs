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

        // trae todas las tutorías del tutorado
        
        [HttpGet("{idTutorado}/historial")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ObtenerHistorialTutorado(
            int idTutorado,
            [FromQuery] List<int>? idsEstado)
        {
            var datos = await _tutoradoService.ObtenerHistorialAsync(idTutorado, idsEstado);
            return Ok(datos);
        }
        
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
        /*[HttpPost("CrearSolicitudTutoria")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> CrearSolicitudTutoria([FromBody] SolicitudTutoriaRequestDto request)
        {
            try
            {
                var result = await _tutoradoService.CrearSolicitudTutoria(request);

                if (result > 0)
                    return Ok("Solicitud de tutoría creada con éxito. Estado: Pendiente");
                else
                    return BadRequest("No se pudo crear la solicitud de tutoría");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }*/
        private static string? Clean(string? s)
            => string.IsNullOrWhiteSpace(s) || s?.Trim().ToLower() == "string" ? null : s;

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

                    // ✅ Devolver el ID al frontend si lo necesitas para navegación
                    return Ok(new { mensaje = "Solicitud de tutoría creada con éxito. ", idTutoria });
                }
                else
                {
                    return BadRequest("No se pudo crear la solicitud de tutoría");
                }
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
        [HttpPost("CrearComentario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CrearComentario([FromBody] CrearComentarioDto dto)
        {
            try
            {
                var mensaje = await _tutoradoService.CrearComentarioAsync(dto);
                return Ok(new { mensaje });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno: " + ex.Message });
            }
        }

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



    }


}



    
   
    