using EduConnect_API.Dtos;

namespace EduConnect_API.Services.Interfaces
{
    public interface ITutorService
    {
        // ✅ idTutor obligatorio, estados opcionales
        Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialAsync(int idTutor, List<int>? estados);

        Task<IEnumerable<ObtenerTutorDto>> ObtenerTutoresAsync(BuscarTutorDto filtros);
        
        Task<int> ActualizarPerfilTutor(EditarPerfilDto tutor);

        Task<IEnumerable<SolicitudTutorDto>> ObtenerSolicitudesTutor(FiltroSolicitudesTutorDto filtro);
        Task<int> AceptarSolicitudTutoria(int idTutoria);
        Task<int> RechazarSolicitudTutoria(int idTutoria);
        Task<DetalleSolicitudTutoriaDto> ObtenerDetalleSolicitud(int idTutoria);
        Task<IEnumerable<MateriaDto>> ObtenerMateriasTutor(int idTutor);
        Task<IEnumerable<ObtenerMateriaDto>> BuscarMateriasAsync(FiltrosMateriaDto filtros);
        Task<IEnumerable<ObtenerMateriaDto>> ListarMateriasAsignadasAsync(int idUsuario);

        Task<SeleccionarGuardarMateriaResultadoDto> SeleccionarYGuardarAsync(SeleccionarGuardarMateriaDto dto);


    }
}
