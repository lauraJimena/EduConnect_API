using EduConnect_API.Dtos;

namespace EduConnect_API.Services.Interfaces
{
    public interface ITutoradoService
    {
        Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialAsync(int idTutorado, List<int>? idsEstado);
       
        Task<int> ActualizarPerfilTutorado(EditarPerfilDto tutorado);
        Task<IEnumerable<SolicitudTutoriaDto>> ObtenerSolicitudesTutorias(FiltroSolicitudesDto filtro);
        Task<IEnumerable<EstadoSolicitudDto>> ObtenerEstadosSolicitud();
    }
}
