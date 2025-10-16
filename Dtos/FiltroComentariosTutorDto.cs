namespace EduConnect_API.Dtos
{
    public class FiltroComentariosTutorDto
    {
        public int IdTutor { get; set; }
        public DateTime? Fecha { get; set; } // Filtro opcional por fecha
        public int? Calificacion { get; set; } // Filtro opcional por calificación (1-5)
    
}
}
