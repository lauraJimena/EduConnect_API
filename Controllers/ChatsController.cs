using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EduConnect_API.Services.Interfaces;
using EduConnect_API.Dtos;


namespace EduConnect_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly ILogger<ChatsController> _logger;
        private readonly IChatsService _chatsService;
        public ChatsController(ILogger<ChatsController> logger, IChatsService chatsService)
        {
            _logger = logger;
            _chatsService = chatsService;
        }
        /// <summary>
        /// Crear un nuevo chat
        /// </summary>
        [HttpPost("CrearChat")]
        public async Task<ActionResult> CrearChat([FromBody] CrearChatDto chat)
        {
            try
            {
                await _chatsService.CrearChat(chat);
                return Ok("Chat registrado con éxito");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
        /// <summary>
        /// Crear un nuevo mensaje en un chat
        /// </summary>
        [HttpPost("CrearMensaje")]
        public async Task<ActionResult> CrearMensaje([FromBody] CrearMensajeDto mensaje)
        {
            try
            {
                await _chatsService.CrearMensaje(mensaje);
                return Ok("Mensaje registrado con éxito");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
        /// <summary>
        /// Obtener chats por usuario
        /// </summary>
        [HttpGet("ObtenerChatsPorUsuario")]
        public async Task<ActionResult> ObtenerChatsPorUsuario(int usuarioId)
        {
            try
            {
                var chats = await _chatsService.ObtenerChatsPorUsuario(usuarioId);
                return Ok(chats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
        /// <summary>
        /// Obtiene los mensajes de un chat específico
        /// </summary>

        [HttpGet("ObtenerMensajes")]
        public async Task<ActionResult> ObtenerMensajes(int chatId)
        {
            try
            {
                var mensajes = await _chatsService.ObtenerMensajes(chatId);
                return Ok(mensajes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }

        }

    }
}
