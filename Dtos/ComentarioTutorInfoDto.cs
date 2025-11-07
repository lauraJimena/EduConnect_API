namespace EduConnect_API.Dtos
{
    public class ComentarioTutorInfoDto
    {
        public int IdComentario { get; set; }
        public string Usuario { get; set; } = string.Empty; // Nombre del tutorado que comentó
        public string Comentario { get; set; } = string.Empty; // Texto del comentario
        public int Calificacion { get; set; } // Calificación (1-5)
        public DateTime Fecha { get; set; } 
        public int IdEstado { get; set; } // 1 = activo, 2 = inactivo
        public string NomEstado { get; set; } = string.Empty;
    }
}
