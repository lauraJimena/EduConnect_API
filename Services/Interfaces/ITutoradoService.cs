using EduConnect_API.Dtos;
using System.Threading.Tasks;

namespace EduConnect_API.Services.Interfaces
{
    public interface ITutoradoService
    {
        Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialAsync(int idTutorado, List<int>? idsEstado);
       
        Task<int> ActualizarPerfilTutorado(EditarPerfilDto tutorado);
        Task<IEnumerable<ObtenerTutorDto>> ObtenerTutoresAsync(BuscarTutorDto filtros);
        Task<IEnumerable<SolicitudTutoriaDto>> ObtenerSolicitudesTutorias(FiltroSolicitudesDto filtro);
        Task<IEnumerable<EstadoSolicitudDto>> ObtenerEstadosSolicitud();
        Task<int> CrearSolicitudTutoria(SolicitudTutoriaRequestDto solicitud);
        
        Task<IEnumerable<RankingTutorDto>> ObtenerRankingTutores();

        Task<IEnumerable<ComentarioTutorInfoDto>> ObtenerComentariosPorTutor(ComentariosTutorRequestDto request);
        
        Task<PerfilTutorDto> ObtenerPerfilTutorAsync(int idTutor);
        Task<ObtenerUsuarioDto> ObtenerTutoradoPorIdAsync(int idTutorado);
        Task <string>CrearComentarioAsync(CrearComentarioDto dto);
        Task<bool> EnviarCorreoConfirmacionTutoriaAsync(int idTutoria);
    }
}
