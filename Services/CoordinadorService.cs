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
        public async Task<int> ActualizarEstadoComentario(int idComentario)
        {        

            // Actualizar el estado (1 = activo, 2 = inactivo)
            int resultado = await _coordinadorRepository.ActualizarEstadoComentario(idComentario);

            if (resultado <= 0)
                throw new Exception("No se pudo actualizar el estado del comentario.");

            return resultado;
        }
 
        //public async Task<IEnumerable<ListaComentariosDto>> ObtenerComentarios()
        //{
        //    return await _coordinadorRepository.ObtenerComentariosAsync();
        //}
        public async Task<IEnumerable<ListaComentariosDto>> ObtenerComentariosAsync(
    string? carrera = null,
    int? semestre = null,
    string? materia = null,
    List<int>? estados = null)
        {
            return await _coordinadorRepository.ObtenerComentariosAsync(carrera, semestre, materia, estados);
        }


    }
}
