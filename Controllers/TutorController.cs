using EduConnect_API.Dtos;
using EduConnect_API.Services;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TutorController : Controller
    {
        private readonly ITutorService _tutorService;
        public TutorController(ITutorService service) => _tutorService = service;


        // Trae historial del tutor filtrado por múltiples estados
        [HttpGet("{idTutor}/historial/")]
        public async Task<IActionResult> ObtenerHistorialFiltradoTutor(
            int idTutor,
            [FromQuery] List<int>? idEstados)
        {
            // Validación obligatoria del tutor
            if (idTutor <= 0)
                return BadRequest("El ID del tutor es obligatorio.");

            var datos = await _tutorService.ObtenerHistorialAsync(idTutor, idEstados);
            return Ok(datos);
        }

        // Ya existente: búsqueda general de tutores
        private static string? Clean(string? s)
            => string.IsNullOrWhiteSpace(s) || s?.Trim().ToLower() == "string" ? null : s;

        [HttpPost("obtener")]
        public async Task<IActionResult> Obtener([FromBody] BuscarTutorDto filtros)
        {
            filtros.Nombre = Clean(filtros.Nombre);
            filtros.MateriaNombre = Clean(filtros.MateriaNombre);
            filtros.Semestre = Clean(filtros.Semestre);
            filtros.CarreraNombre = Clean(filtros.CarreraNombre);
            if (filtros.IdEstado.HasValue && filtros.IdEstado.Value <= 0) filtros.IdEstado = null;

            var resultado = await _tutorService.ObtenerTutoresAsync(filtros);
            return Ok(resultado);
        }
        [HttpPut("ActualizarPerfil")]
        public async Task<ActionResult> ActualizarPerfil([FromBody] EditarPerfilDto perfil)
        {
            try
            {
                var result = await _tutorService.ActualizarPerfilTutor(perfil);

                if (result > 0)
                    return Ok("Perfil del tutor actualizado con éxito");
                else
                    return NotFound("Tutor no encontrado o no se pudo actualizar");
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
        public async Task<ActionResult<IEnumerable<SolicitudTutorDto>>> ObtenerSolicitudesTutorias([FromBody] FiltroSolicitudesTutorDto filtro)
        {
            try
            {
                var solicitudes = await _tutorService.ObtenerSolicitudesTutor(filtro);
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

        // ACEPTAR solicitud - SOLO id_tutoria
        [HttpPut("AceptarSolicitudTutoria")]
        public async Task<ActionResult> AceptarSolicitudTutoria([FromBody] ActualizarEstadoSolicitudDto request)
        {
            try
            {
                var result = await _tutorService.AceptarSolicitudTutoria(request.IdTutoria);

                if (result > 0)
                    return Ok("Solicitud de tutoría aceptada con éxito");
                else
                    return BadRequest("No se pudo aceptar la solicitud de tutoría (posiblemente ya fue procesada)");
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

        // RECHAZAR solicitud - SOLO id_tutoria
        [HttpPut("RechazarSolicitudTutoria")]
        public async Task<ActionResult> RechazarSolicitudTutoria([FromBody] ActualizarEstadoSolicitudDto request)
        {
            try
            {
                var result = await _tutorService.RechazarSolicitudTutoria(request.IdTutoria);

                if (result > 0)
                    return Ok("Solicitud de tutoría rechazada con éxito");
                else
                    return BadRequest("No se pudo rechazar la solicitud de tutoría (posiblemente ya fue procesada)");
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

        // DETALLE de solicitud - SOLO id_tutoria en la ruta
        [HttpGet("DetalleSolicitud/{idTutoria}")]
        public async Task<ActionResult<DetalleSolicitudTutoriaDto>> ObtenerDetalleSolicitud(int idTutoria)
        {
            try
            {
                var detalle = await _tutorService.ObtenerDetalleSolicitud(idTutoria);
                return Ok(detalle);
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

        // MATERIAS para filtros
        [HttpGet("MateriasTutor/{idTutor}")]
        public async Task<ActionResult<IEnumerable<MateriaDto>>> ObtenerMateriasTutor(int idTutor)
        {
            try
            {
                var materias = await _tutorService.ObtenerMateriasTutor(idTutor);
                return Ok(materias);
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
    }
}