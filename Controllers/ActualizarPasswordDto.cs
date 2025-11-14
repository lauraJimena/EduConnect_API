namespace EduConnect_API.Controllers
{
    public class ActualizarPasswordDto
    {
        public int IdUsuario { get; set; }
        public string NuevaPassword { get; set; }
    }
}