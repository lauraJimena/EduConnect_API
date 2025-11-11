
namespace EduConnect_API.Dtos
{
    public class ComentarioResponseDto
    {
        public int IdComentario { get; set; }
        public string Texto { get; set; } = string.Empty;
        public int Calificacion { get; set; }
        public DateTime Fecha { get; set; }
        public int IdTutor { get; set; }
        public int IdTutorado { get; set; }
        public int IdEstado { get; set; }
        public string NombreTutor { get; set; } = string.Empty;
        public string NombreTutorado { get; set; } = string.Empty;
    }
}
