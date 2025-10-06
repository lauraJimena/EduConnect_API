using EduConnect_API.Dtos;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface ITutorRepository
    {
        Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialTutorAsync(int idTutor, List<int>? estados);

        Task<IEnumerable<ObtenerTutorDto>> ObtenerTutoresAsync(BuscarTutorDto filtros);

    }
}
