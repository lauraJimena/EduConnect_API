namespace EduConnect_API.Dtos
{
    public class ComentarioTutorInfoDto
    {
        public string Usuario { get; set; } = string.Empty; // Nombre del tutorado que comentó
        public string Comentario { get; set; } = string.Empty; // Texto del comentario
        public int Calificacion { get; set; } // Calificación (1-5)
        public DateTime Fecha { get; set; } // Fecha del com
    }
}
