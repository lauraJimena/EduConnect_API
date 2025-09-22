using EduConnect_API.Dtos;
using EduConnect_API.Services;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {

        private readonly ILogger<UsuarioController> _logger;
        private readonly IUsuarioService _usuarioService;
        public UsuarioController(ILogger<UsuarioController> logger, IUsuarioService usuarioService)
        {
            _logger = logger;
            _usuarioService = usuarioService;
        }
       
        [HttpPost("RegistrarUsuario")]
        public async Task<ActionResult> RegistrarUsuario([FromBody] UsuarioDto usuario)
        {
            try
            {
                await _usuarioService.RegistrarUsuario(usuario);

                return Ok("Usuario registrado con éxito");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
        [HttpGet("ObtenerUsuarios")]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> ObtenerUsuarios()
        {
            
            var usuarios = await _usuarioService.ObtenerUsuarios();

            
            return Ok(usuarios);
        }

        //private static readonly string[] Summaries = new[]
        //{
        //    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        //};

        //private readonly ILogger<UsuarioController> _logger;
        //private readonly IUsuarioService _usuarioService;
        //public UsuarioController(ILogger<UsuarioController> logger, IUsuarioService usuarioService)
        //{
        //    _logger = logger;
        //    _usuarioService = usuarioService;
        //}

        //[HttpGet("ObtenerUsuarios")]
        //public IEnumerable<UsuarioDto> ObtenerUsuarios()
        //{
        //    return _usuarioService.ObtenerUsuarios();
        //}

        //[HttpGet("ConsultarUsuario/{id}")]
        //public ActionResult<UsuarioDto> GetById(int id)
        //{
        //    if (id < 1 || id > 5) // Solo generamos entre 1 y 5
        //    {
        //        return NotFound("No existe un pronóstico con ese id");
        //    }

        //    var forecast = new UsuarioDto
        //    {
        //        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(id)),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    };

        //    return Ok(forecast);
        //}

    }
}
