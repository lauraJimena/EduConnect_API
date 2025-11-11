namespace EduConnect_API.Dtos
{
    public class FiltroComentariosTutorDto
    {
        public int IdTutor { get; set; }
        public int? OrdenFecha { get; set; } // 1 = Más recientes primero, 2 = Más antiguos primero
        public int? Calificacion { get; set; } // Filtro opcional por calificación (1-5)
    }

}
