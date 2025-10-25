using EduConnect_API.Dtos;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface ICoordinadorRepository
    {

        Task<IEnumerable<TutoriaConsultaDto>> ConsultarTutoriasAsync(string? carrera, int? semestre, string? materia, int? idEstado, int? ordenFecha);
        Task<(ReporteGestionAdministrativaDto Totales, List<ReporteDesempenoTutorDto> Desempeno)> ObtenerReporteCombinadoAsync();
        Task<ReporteDemandaAcademicaDto> ObtenerReporteDemandaAcademicaAsync();
    }
}
