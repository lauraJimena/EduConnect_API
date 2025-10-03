using EduConnect_API.Dtos;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class TutorService : ITutorService
    {
        private readonly ITutorRepository _tutorRepository;

        public TutorService(ITutorRepository repo) => _tutorRepository = repo;

        public Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialAsync(int idTutor)
            => _tutorRepository.ObtenerHistorialTutorAsync(idTutor);


    }
}

