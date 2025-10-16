using EduConnect_API.Dtos;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface ITutorRepository
    {
        Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialTutorAsync(int idTutor, List<int>? estados);

      
        Task<int> ActualizarPerfilTutor(EditarPerfilDto perfil);
        Task<bool> ExisteUsuario(int idUsuario);
        Task<int> ObtenerRolUsuario(int idUsuario);

        Task<IEnumerable<SolicitudTutorDto>> ObtenerSolicitudesTutor(FiltroSolicitudesTutorDto filtro);
        Task<int> AceptarSolicitudTutoria(int idTutoria);
        Task<int> RechazarSolicitudTutoria(int idTutoria);
        Task<DetalleSolicitudTutoriaDto> ObtenerDetalleSolicitud(int idTutoria);
        Task<IEnumerable<MateriaDto>> ObtenerMateriasTutor(int idTutor);

        Task<IEnumerable<ObtenerMateriaDto>> BuscarMateriasAsync(FiltrosMateriaDto filtros);
        Task<IEnumerable<ObtenerMateriaDto>> ListarMateriasAsignadasAsync(int idUsuario);
        Task<SeleccionarGuardarMateriaResultadoDto> SeleccionarYGuardarAsync(SeleccionarGuardarMateriaDto dto);

        Task<bool> ExisteMateria(int idMateria);
        Task<int> ObtenerCarreraPorMateria(int idMateria);
        Task<int> ContarMateriasAsignadas(int idUsuario);
        Task<bool> ExisteAsignacion(int idUsuario, int idMateria);
        Task<int?> ObtenerCarreraDeUsuario(int idUsuario);

        Task<IEnumerable<ComentarioTutorDto>> ObtenerComentariosTutor(FiltroComentariosTutorDto filtro);


    }
}
