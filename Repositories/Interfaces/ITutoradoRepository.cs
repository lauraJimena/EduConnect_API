using EduConnect_API.Dtos;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface ITutoradoRepository
    {
        Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialTutoradoAsync(int idTutorado, List<int>? idsEstado);
        Task<int> ActualizarPerfil(EditarPerfilDto perfil);
        Task<bool> ExisteUsuario(int idUsuario);
        Task<int> ObtenerRolUsuario(int idUsuario);
        Task<IEnumerable<SolicitudTutoriaDto>> ObtenerSolicitudesTutorias(FiltroSolicitudesDto filtro);
        Task<IEnumerable<EstadoSolicitudDto>> ObtenerEstadosSolicitud();
    }
}
