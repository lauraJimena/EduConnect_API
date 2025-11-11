namespace EduConnect_API.Dtos
{
    public class ComentarioAdvertenciaDto
    {
        public int IdComentario { get; set; }
        public string Texto { get; set; } = string.Empty;
        public int Calificacion { get; set; }
        public string Materia { get; set; } = string.Empty;
        public string NombreTutor { get; set; } = string.Empty;
        public string NombreTutorado { get; set; } = string.Empty;
        public string CorreoTutor { get; set; } = string.Empty;
    }
}
