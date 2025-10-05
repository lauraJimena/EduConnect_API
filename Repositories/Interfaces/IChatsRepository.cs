using EduConnect_API.Dtos;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface IChatsRepository
    {
        Task<int> CrearChat(CrearChatDto chat);
        Task<int> CrearMensaje(CrearMensajeDto mensaje);
        Task<IEnumerable<ObtenerChatDto>> ObtenerChatsPorUsuario(int usuarioId);
        Task<IEnumerable<ObtenerMensajeDto>> ObtenerMensajes(int idChat);

    }
}
