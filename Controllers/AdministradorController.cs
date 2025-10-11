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
    public class AdministradorController : ControllerBase
    {

        private readonly ILogger<AdministradorController> _logger;
        private readonly IAdministradorService _administradorService;
        public AdministradorController(ILogger<AdministradorController> logger, IAdministradorService administradorService)
        {
            _logger = logger;
            _administradorService = administradorService;
        }
         /// <summary>
        /// Registra usarios 
        /// </summary>
        [HttpPost("RegistrarUsuario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> RegistrarUsuario([FromBody] CrearUsuarioDto usuario)
        {
            try
            {
                await _administradorService.RegistrarUsuario(usuario);

                return Ok("Usuario registrado con éxito");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
        //[HttpGet("ConsultarUsuarios")]
        //public async Task<ActionResult<IEnumerable<ObtenerUsuarioDto>>> ObtenerUsuarios()
        //{
        //    try
        //    {

        //        var usuarios = await _administradorService.ObtenerUsuarios();

        //        return Ok(usuarios);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Error interno: " + ex.Message);
        //    }
        //}
        /// <summary>
        /// Obtiene la lista de todos los usuarios registrados en el sistema
        /// </summary>
        [HttpGet("ConsultarUsuarios")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ConsultarUsuarios(int? idRol, int? idEstado, string? numIdent)
        {
            try
            {
                var usuarios = await _administradorService.ObtenerUsuariosAsync(idRol, idEstado, numIdent);
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }

        //Obtener usuario por ID    
        [HttpGet("ObtenerUsuarioPorId/{idUsuario}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ObtenerUsuarioPorId(int idUsuario)
        {
            try
            {
                var usuario = await _administradorService.ObtenerUsuarioPorId(idUsuario);
                return Ok(usuario);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("ActualizarUsuario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ActualizarUsuario([FromBody] ActualizarUsuarioDto usuario)
        {
            try
            {
                var result = await _administradorService.ActualizarUsuario(usuario);

                if (result > 0)
                    return Ok("Usuario actualizado con éxito");
                else
                    return NotFound("Usuario no encontrado o no se pudo actualizar");
            }
            catch (ArgumentException ex) // validaciones del service
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
        [HttpDelete("EliminarUsuario/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> EliminarUsuario(int id)
        {
            try
            {
                await _administradorService.EliminarUsuario(id);
                return Ok("Usuario inactivado con éxito");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
       


    }
}
