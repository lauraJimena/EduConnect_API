namespace EduConnect_API.Dtos
{
    public class ComentarioTutorDto
    {
        public int IdComentario { get; set; }
        public string Usuario { get; set; } = string.Empty; // Nombre del tutorado
        public string Descripcion { get; set; } = string.Empty; // Texto del comentario
        public DateTime Fecha { get; set; }
        public string FechaFormateada { get; set; } = string.Empty; // Fecha en formato amigable
        public int Calificacion { get; set; }
        
    
}
}
