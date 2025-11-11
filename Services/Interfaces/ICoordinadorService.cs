
using EduConnect_API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Services.Interfaces
{
    public interface ICoordinadorService
    {
        //Task<IEnumerable<TutoriaConsultaDto>>ConsultarTutorias(string? carrera, int? semestre, string? materia, DateTime? fecha, string? estados);
        Task<IEnumerable<TutoriaConsultaDto>> ConsultarTutoriasAsync(string? carrera, int? semestre, string? materia, int? idEstado, int? ordenFecha);
        //Task<(object totales, object desempeno)> ObtenerReporteCombinadoAsync();
        Task<ReporteDemandaAcademicaDto> ObtenerReporteDemandaAcademicaAsync();
        Task<(ReporteGestionAdministrativaDto Totales, List<ReporteDesempenoTutorDto> Desempeno)>
          ObtenerReporteCombinadoAsync();
        Task<int> ActualizarEstadoComentario(int idComentario);

        //Task<IEnumerable<ListaComentariosDto>> ObtenerComentarios();
        Task<IEnumerable<ListaComentariosDto>> ObtenerComentariosAsync(
    string? carrera = null,
    int? semestre = null,
    string? materia = null,
    List<int>? estados = null);

    }

}
