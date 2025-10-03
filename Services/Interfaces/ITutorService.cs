using EduConnect_API.Dtos;

namespace EduConnect_API.Services.Interfaces
{
    public interface ITutorService
    {
        Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialAsync(int idTutor);
    }
}

