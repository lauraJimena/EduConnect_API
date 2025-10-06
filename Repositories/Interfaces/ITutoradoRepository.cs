using EduConnect_API.Dtos;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface ITutoradoRepository
    {
        Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialTutoradoAsync(int idTutorado, List<int>? idsEstado);
    }
}
