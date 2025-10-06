using EduConnect_API.Dtos;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;
using EduConnect_API.Dtos;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class TutoradoService : ITutoradoService
    {
        private readonly ITutoradoRepository _repo;

        public TutoradoService(ITutoradoRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialAsync(int idTutorado, List<int>? idsEstado)
            => _repo.ObtenerHistorialTutoradoAsync(idTutorado, idsEstado);
    }
}
