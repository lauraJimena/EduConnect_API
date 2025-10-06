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
    }
}
