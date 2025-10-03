using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TutorController : Controller
    {
        private readonly ILogger<TutorController> _logger;
        private readonly ITutorService _tutorService;
        public TutorController(ILogger<TutorController> logger, ITutorService tutorService)
        {
            _logger = logger;
            _tutorService = tutorService;
        }

    }
}
