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
    }
}
