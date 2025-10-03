using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class TutoradoService : ITutoradoService
    {
        private readonly ITutoradoRepository _tutoradoRepository;

        public TutoradoService(ITutoradoRepository tutoradoRepository)
        {
            _tutoradoRepository = tutoradoRepository;
        }
    }
}
