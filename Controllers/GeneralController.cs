using EduConnect_API.Dtos;
using EduConnect_API.Services;
using EduConnect_API.Services.Interfaces;
using EduConnect_API.Utilities;
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
                int idGenerado = await _generalService.RegistrarUsuario(usuario);

                var response = new
                {
                    ok = true,
                    msg = "Usuario registrado con éxito",
                    idUsu = idGenerado
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    ok = false,
                    msg = "Error interno: " + ex.Message
                });
            }
        }

        [HttpPost("EnviarBienvenida")]
        public async Task<IActionResult> EnviarCorreoBienvenida([FromQuery] int idUsu)
        {
            try
            {
                var resultado = await _generalService.EnviarCorreoBienvenidaAsync(idUsu);

                if (resultado)
                    return Ok("✅ Correo enviado correctamente.");
                else
                    return BadRequest("❌ No se pudo enviar el correo.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error en el servidor: {ex.Message}");
            }
        }
        /// <summary>
        /// Inicia sesión para un usuario mediante correo y contraseña.
        /// </summary>
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
        /// <summary>
        /// Obtiene todas las carreras de la base de datos
        /// </summary>
        [HttpGet("ObtenerCarreras")]
        public async Task<ActionResult> ObtenerCarreras()
        {
            try
            {
                var carreras = await _generalService.ObtenerCarrerasAsync();

                if (carreras == null || !carreras.Any())
                    return NotFound("No se encontraron carreras registradas.");

                return Ok(carreras);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
        [HttpGet("ObtenerTiposIdent")]
        public async Task<ActionResult> ObtenerTipoIdent()
        {
            try
            {
                var carreras = await _generalService.ObtenerTiposIdentidadAsync();

                if (carreras == null || !carreras.Any())
                    return NotFound("No se encontraron carreras registradas.");

                return Ok(carreras);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
        [HttpPost("ActualizarPassword")]
        public async Task<IActionResult> ActualizarPassword([FromBody] ActualizarPasswordDto dto)
        {
            try
            {
                // Validar parámetros básicos
                if (dto == null || dto.IdUsuario <= 0)
                    return BadRequest(new { mensaje = "El ID de usuario es obligatorio." });

                if (string.IsNullOrWhiteSpace(dto.NuevaPassword))
                    return BadRequest(new { mensaje = "Debe proporcionar una nueva contraseña." });

                // Ejecutar la lógica en el service
                var resultado = await _generalService.ActualizarPasswordAsync(dto.IdUsuario, dto.NuevaPassword);

                if (resultado > 0)
                    return Ok(new { mensaje = "Contraseña actualizada correctamente." });

                // Si no se afectó ninguna fila, devolver un error genérico
                return BadRequest(new { mensaje = "No se pudo actualizar la contraseña. Verifique los datos enviados." });
            }
            catch (Exception ex)
            {
                // Manejo de errores controlado
                return StatusCode(500, new { mensaje = "Error interno: " + ex.Message });
            }
        }
        [HttpPost("EnviarCorreoRecuperacion")]
        public async Task<IActionResult> EnviarCorreoRecuperacion([FromQuery] string correo)
        {
            try
            {
                var resultado = await _generalService.EnviarCorreoRecuperacionAsync(correo);

                if (resultado)
                    return Ok("✅ Se ha enviado el enlace de recuperación al correo indicado.");
                else
                    return BadRequest("❌ No se pudo enviar el correo. Verifique el correo ingresado.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost("RestablecerContrasena")]
        public async Task<IActionResult> RestablecerContrasena([FromBody] RestablecerContrasenaDto dto)
        {   
            try
            {
                var (ok, msg) = await _generalService.RestablecerContrasenaAsync(dto);

                if (ok)
                    return Ok(new { mensaje = msg });

                return BadRequest(new { mensaje = msg });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno: " + ex.Message });
            }
        }




    }
}
