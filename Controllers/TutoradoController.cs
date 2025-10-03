using EduConnect_API.Controllers;
using EduConnect_API.Services;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TutoradoController : Controller
    {
        private readonly ITutoradoService _tutoradoService;

        public TutoradoController(ITutoradoService tutoradoService) => _tutoradoService = tutoradoService;

      // trae todas las tutorias del tutorado
        [HttpGet("{idTutorado}/historial")]
        public async Task<IActionResult> ObtenerHistorialTutorado(int idTutorado)
        {
            var datos = await _tutoradoService.ObtenerHistorialAsync(idTutorado);
            return Ok(datos); // devolverá [] si no hay coincidencias
        }
    }
}