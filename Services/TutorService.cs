using EduConnect_API.Dtos;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class TutorService : ITutorService
    {
        private readonly ITutorRepository _tutorRepository;

        public TutorService(ITutorRepository repo) => _tutorRepository = repo;

        // ✅ Validar que el id del tutor sea obligatorio
        public async Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialAsync(int idTutor, List<int>? estados)
        {
            if (idTutor <= 0)
                throw new ArgumentException("El ID del tutor es obligatorio y debe ser mayor que 0.");

            return await _tutorRepository.ObtenerHistorialTutorAsync(idTutor, estados);
        }

        public Task<IEnumerable<ObtenerTutorDto>> ObtenerTutoresAsync(BuscarTutorDto filtros)
            => _tutorRepository.ObtenerTutoresAsync(filtros);
    }

}
