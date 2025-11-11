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
    public class TutorController : Controller
    {
        private readonly ITutorService _tutorService;
        public TutorController(ITutorService service) => _tutorService = service;


        // Trae historial del tutor filtrado por múltiples estados
        [HttpGet("{idTutor}/historial/")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        
        [HttpPut("ActualizarPerfil")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        // 🔹 Buscar materias por filtros (nombre, semestre, carrera)
        //[HttpPost("BuscarMaterias")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<IActionResult> BuscarMaterias([FromBody] FiltrosMateriaDto filtros)
        //{
        //    filtros.MateriaNombre = Clean(filtros.MateriaNombre);
        //    filtros.Semestre = Clean(filtros.Semestre);
        //    filtros.CarreraNombre = Clean(filtros.CarreraNombre);

        //    var resultado = await _tutorService.BuscarMateriasAsync(filtros);
        //    return Ok(resultado);
        //}

        // 🔹 Listar las materias ya asignadas al tutor
        [HttpGet("ObtenerAsignadas")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ObtenerAsignadas(int idUsuario)
        {
            var resultado = await _tutorService.ListarMateriasAsignadasAsync(idUsuario);
            return Ok(resultado);
        }

        // 🔹 Guardar selección (máximo 5 materias)
        [HttpPost("SeleccionarGuardarMateria")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SeleccionarGuardarMateria([FromBody] SeleccionarGuardarMateriaDto dto)
        {
            

            var resultado = await _tutorService.SeleccionarYGuardarAsync(dto);
            return Ok(resultado);
        }
        [HttpPost("Comentarios")]
        public async Task<ActionResult<IEnumerable<ComentarioTutorDto>>> ObtenerComentariosTutor([FromBody] FiltroComentariosTutorDto filtro)
        {
            try
            {
                var comentarios = await _tutorService.ObtenerComentariosTutor(filtro);
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
        [HttpGet("ObtenerTutorPorId/{idUsuario}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ObtenerTutoradoPorId(int idUsuario)
        {
            try
            {
                // Llama al servicio
                var usuario = await _tutorService.ObtenerTutorPorIdAsync(idUsuario);

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
        [HttpGet("ValidarMaterias/{idTutor}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ValidarMateriasTutor(int idTutor)
        {
            try
            {
                bool tieneMaterias = await _tutorService.ValidarMateriasTutorAsync(idTutor);

                return Ok(new
                {
                    tieneMaterias = tieneMaterias,
                    mensaje = tieneMaterias
                        ? "El tutor ya tiene materias registradas."
                        : "El tutor no tiene materias registradas."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno al validar materias: " + ex.Message });
            }
        }
        // GET: Tutor/RegistrarMaterias
        [HttpGet("MateriasPorTutor/{idTutor}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ObtenerMateriasPorTutor(int idTutor)
        {
            try
            {
                var materias = await _tutorService.ObtenerMateriasPorTutorAsync(idTutor);

                if (materias == null )
                    return NotFound(new { mensaje = "No se encontraron materias disponibles para este tutor." });

                return Ok(materias);
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
        [HttpPost("RegistrarMaterias")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RegistrarMaterias([FromBody] RegistrarMateriasTutorDto dto)
        {
            try
            {
                await _tutorService.RegistrarMateriasTutorAsync(dto);
                return Ok(new { message = "Materias registradas correctamente." });
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




    }
}