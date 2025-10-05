using EduConnect_API.Dtos;

namespace EduConnect_API.Services.Interfaces
{
    public interface IChatsService
    {
        Task CrearChat(CrearChatDto chat);
        Task CrearMensaje(CrearMensajeDto mensaje);
        Task<IEnumerable<ObtenerChatDto>> ObtenerChatsPorUsuario(int idUsuario);
        Task<IEnumerable<ObtenerMensajeDto>> ObtenerMensajes(int idChat);
    }
}
