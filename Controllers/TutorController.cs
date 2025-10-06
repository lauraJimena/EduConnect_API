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
    }
}

