namespace EduConnect_API.Dtos
{
    public class CrearComentarioDto
    {
        public string Texto { get; set; } = string.Empty;
        public int Calificacion { get; set; }
        public int IdTutor { get; set; }
        public int IdTutorado { get; set; }
        public int IdEstado { get; set; }
    }
}

