using EduConnect_API.Dtos;
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

        // GET api/tutor/{id}/historial
        [HttpGet("{idTutor}/historial")]
        public async Task<IActionResult> ObtenerHistorialTutor(int idTutor)
        {
            var datos = await _tutorService.ObtenerHistorialAsync(idTutor);
            return Ok(datos);
        }
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
    }
}
