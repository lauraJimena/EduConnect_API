namespace EduConnect_API.Dtos
{
    public class SolicitudTutoriaRequestDto
    {

        public int IdTutorado { get; set; }
        public int IdTutor { get; set; }
        public int IdMateria { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; } = string.Empty; // Cambiado a string
        public int IdModalidad { get; set; }
        public string Tema { get; set; } = string.Empty;
        public string? ComentarioAdicional { get; set; }
    }
    }

