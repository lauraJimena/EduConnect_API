using EduConnect_API.Dtos;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface ICoordinadorRepository
    {
        Task<int> ActualizarEstadoComentario(int idComentario);
        Task<IEnumerable<TutoriaConsultaDto>> ConsultarTutoriasAsync(string? carrera, int? semestre, string? materia, int? idEstado, int? ordenFecha);
        //Task<IEnumerable<ListaComentariosDto>> ObtenerComentariosAsync(string? carrera);
        Task<IEnumerable<ListaComentariosDto>> ObtenerComentariosAsync(
   string? carrera = null,
   int? semestre = null,
   string? materia = null,
   List<int>? estados = null);
        Task<(ReporteGestionAdministrativaDto Totales, List<ReporteDesempenoTutorDto> Desempeno)> ObtenerReporteCombinadoAsync();
        Task<ReporteDemandaAcademicaDto> ObtenerReporteDemandaAcademicaAsync();
    }
}
