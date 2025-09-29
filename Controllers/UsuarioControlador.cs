using EduConnect_API.Dtos;
using EduConnect_API.Servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controladores
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioControlador : ControllerBase
    {
        private readonly IUsuarioServicio _usuarioServicio;

        public UsuarioControlador(IUsuarioServicio usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
        }

        [HttpPost("iniciar-sesion")]
        public async Task<IActionResult> IniciarSesion([FromBody] IniciarSesion dto)
        {
            var resultado = await _usuarioServicio.IniciarSesion(dto);

            if (resultado == null)
            {
                return Unauthorized("Número de identificación o contraseña incorrectos.");
            }

            return Ok(resultado);
        }
    }
}
