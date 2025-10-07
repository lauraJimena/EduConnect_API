using EduConnect_API.Dtos;

namespace EduConnect_API.Services.Interfaces
{
    public interface ITutorService
    {
        // ✅ idTutor obligatorio, estados opcionales
        Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialAsync(int idTutor, List<int>? estados);

        Task<IEnumerable<ObtenerTutorDto>> ObtenerTutoresAsync(BuscarTutorDto filtros);
        
        Task<int> ActualizarPerfilTutor(EditarPerfilDto tutor);

    }
}
