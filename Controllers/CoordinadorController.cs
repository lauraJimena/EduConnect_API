using EduConnect_API.Dtos;
using EduConnect_API.Services;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoordinadorController: ControllerBase
    {
        private readonly ILogger<CoordinadorController> _logger;
        private readonly ICoordinadorService _coordinadorService;
        public CoordinadorController(ILogger<CoordinadorController> logger, ICoordinadorService coordinadorService)
        {
            _logger = logger;
            _coordinadorService = coordinadorService;
        }
        [HttpGet("ConsultarTutorias")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<TutoriaConsultaDto>>> ConsultarTutorias(
    [FromQuery] string? carrera,
    [FromQuery] int? semestre,
    [FromQuery] string? materia,
    [FromQuery] int? idEstado,
    [FromQuery] int? ordenFecha)
        {
            try
            {
                var resultado = await _coordinadorService.ConsultarTutoriasAsync(
                    carrera, semestre, materia, idEstado, ordenFecha);

                if (!resultado.Any())
                    return NotFound("No se encontraron tutorías con los filtros especificados.");

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
        [HttpGet("ReporteDemandaAcademica")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ReporteDemandaAcademicaDto>> ObtenerReporteDemandaAcademica()
        {
            try
            {
                var reporte = await _coordinadorService.ObtenerReporteDemandaAcademicaAsync();

                if (reporte == null)
                    return NotFound("No se encontraron datos para generar el reporte.");

                return Ok(reporte);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al generar el reporte: {ex.Message}");
            }
        }
        [HttpGet("ReporteCombinado")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ObtenerReporteCombinado()
        {
            try
            {
                var (totales, desempeno) = await _coordinadorService.ObtenerReporteCombinadoAsync();

                var resultado = new
                {
                    Totales = totales,
                    Desempeno = desempeno
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al generar el reporte combinado: {ex.Message}");
            }
        }



        }
}
