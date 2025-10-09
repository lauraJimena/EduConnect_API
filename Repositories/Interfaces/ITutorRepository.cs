using EduConnect_API.Dtos;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface ITutorRepository
    {
        Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialTutorAsync(int idTutor, List<int>? estados);

        Task<IEnumerable<ObtenerTutorDto>> ObtenerTutoresAsync(BuscarTutorDto filtros);
        Task<int> ActualizarPerfilTutor(EditarPerfilDto perfil);
        Task<bool> ExisteUsuario(int idUsuario);
        Task<int> ObtenerRolUsuario(int idUsuario);

        Task<IEnumerable<SolicitudTutorDto>> ObtenerSolicitudesTutor(FiltroSolicitudesTutorDto filtro);
        Task<int> AceptarSolicitudTutoria(int idTutoria);
        Task<int> RechazarSolicitudTutoria(int idTutoria);
        Task<DetalleSolicitudTutoriaDto> ObtenerDetalleSolicitud(int idTutoria);
        Task<IEnumerable<MateriaDto>> ObtenerMateriasTutor(int idTutor);

    }
}
