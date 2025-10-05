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
