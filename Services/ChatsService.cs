using EduConnect_API.Dtos;
using EduConnect_API.Repositories;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class ChatsService : IChatsService
    {
        private readonly IChatsRepository _chatsRepository;

        public ChatsService(IChatsRepository chatsRepository)
        {
            _chatsRepository = chatsRepository;
        }

        public async Task CrearChat(CrearChatDto chat)
        {
            var result = await _chatsRepository.CrearChat(chat);
            if (result <= 0)
                throw new Exception("No se pudo registrar el chat en la base de datos.");
        }

        public async Task CrearMensaje(CrearMensajeDto mensaje)
        {
            var result = await _chatsRepository.CrearMensaje(mensaje);
            if (result <= 0)
                throw new Exception("No se pudo registrar el mensaje en la base de datos.");
        }

        public async Task<IEnumerable<ObtenerChatDto>> ObtenerChatsPorUsuario(int IdUsuario)
        {
            var chats = await _chatsRepository.ObtenerChatsPorUsuario(IdUsuario);


            if (chats == null || !chats.Any())
            {
                // En vez de devolver null, lanza excepción controlada
                throw new Exception("No se encontraron usuarios en la base de datos.");
            }

            return chats;
        }

        public async Task<IEnumerable<ObtenerMensajeDto>> ObtenerMensajes(int idChat)
        {
            var mensajes = await _chatsRepository.ObtenerMensajes(idChat);
            if (mensajes == null || !mensajes.Any())
            {
                // En vez de devolver null, lanza excepción controlada
                throw new Exception("No se encontraron mensajes en la base de datos.");
            }
            return mensajes;
        }
    }
}
