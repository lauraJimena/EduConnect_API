using EduConnect_API.Dtos;
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

        public TutoradoController(ITutoradoService tutoradoService)
        {
            _tutoradoService = tutoradoService;
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
        [HttpPost("CrearSolicitudTutoria")]
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
        }
    }

        
    }



    
   
    