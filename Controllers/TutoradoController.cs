using EduConnect_API.Dtos;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TutoradoController : Controller
    {
        private readonly ITutoradoService _tutoradoService;

        public TutoradoController(ITutoradoService tutoradoService)
        {
            _tutoradoService = tutoradoService;
        }

        // trae todas las tutorías del tutorado
        [HttpGet("{idTutorado}/historial")]
        public async Task<IActionResult> ObtenerHistorialTutorado(
            int idTutorado,
            [FromQuery] List<int>? idsEstado)
        {
            var datos = await _tutoradoService.ObtenerHistorialAsync(idTutorado, idsEstado);
            return Ok(datos);
        }
        [HttpPut("ActualizarPerfil")]
        public async Task<ActionResult> ActualizarPerfil([FromBody] EditarPerfilDto perfil)
        {
            try
            {
                var result = await _tutoradoService.ActualizarPerfilTutorado(perfil);

                if (result > 0)
                    return Ok("Perfil del tutorado actualizado con éxito");
                else
                    return NotFound("Tutorado no encontrado o no se pudo actualizar");
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
    }
}
   
    