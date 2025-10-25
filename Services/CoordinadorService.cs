using EduConnect_API.Dtos;
using EduConnect_API.Repositories;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class CoordinadorService : ICoordinadorService
    {
        private readonly ICoordinadorRepository _coordinadorRepository;

        public CoordinadorService(ICoordinadorRepository coordinadorRepository)
        {
            _coordinadorRepository = coordinadorRepository;
        }
        public async Task<IEnumerable<TutoriaConsultaDto>> ConsultarTutoriasAsync(
    string? carrera, int? semestre, string? materia, int? idEstado, int? ordenFecha)
        {
            return await _coordinadorRepository.ConsultarTutoriasAsync(
                carrera, semestre, materia, idEstado, ordenFecha);
        }
        public async Task<ReporteDemandaAcademicaDto> ObtenerReporteDemandaAcademicaAsync()
        {
            return await _coordinadorRepository.ObtenerReporteDemandaAcademicaAsync();
        }
        public async Task<(ReporteGestionAdministrativaDto Totales, List<ReporteDesempenoTutorDto> Desempeno)>
           ObtenerReporteCombinadoAsync()
        {
            return await _coordinadorRepository.ObtenerReporteCombinadoAsync();
        }


    }
}
