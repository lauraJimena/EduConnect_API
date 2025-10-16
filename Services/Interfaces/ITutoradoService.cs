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
        Task<ComentarioResponseDto> CrearComentario(CrearComentarioDto comentario);
        Task<IEnumerable<RankingTutorDto>> ObtenerRankingTutores();

        Task<IEnumerable<ComentarioTutorInfoDto>> ObtenerComentariosPorTutor(ComentariosTutorRequestDto request);
    }
}
