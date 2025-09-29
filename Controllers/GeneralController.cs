using EduConnect_API.Dtos;
using EduConnect_API.Services;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace EduConnect_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        private readonly ILogger<GeneralController> _logger;
        private readonly IGeneralService _generalService;
        public GeneralController(ILogger<GeneralController> logger, IGeneralService generalService)
        {
            _logger = logger;
            _generalService = generalService;
        }
        [HttpPost("RegistrarUsuario")]
        public async Task<ActionResult> RegistrarUsuario([FromBody] CrearUsuarioDto usuario)
        {
            try
            {
                await _generalService.RegistrarUsuario(usuario);

                return Ok("Usuario registrado con éxito");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
        [HttpPost("IniciarSesión")]
        public async Task<IActionResult> IniciarSesion([FromBody] IniciarSesionDto usuario)
        {
            try
            {
                var resultado = await _generalService.IniciarSesion(usuario);


                return Ok(resultado);
            
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
    }
}
