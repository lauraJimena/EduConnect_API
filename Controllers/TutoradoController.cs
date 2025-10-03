using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TutoradoController : Controller
    {
        private readonly ILogger<TutoradoController> _logger;
        private readonly ITutoradoService _tutoradoService;
        public TutoradoController(ILogger<TutoradoController> logger, ITutoradoService tutoradoService)
        {
            _logger = logger;
            _tutoradoService = tutoradoService;
        }
    }
}
