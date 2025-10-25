namespace EduConnect_API.Dtos
{
    public class DatosCorreoTutoriaDto
    {
        public string NombreTutorado { get; set; } = string.Empty;
        public string CorreoTutorado { get; set; } = string.Empty;
        public string NombreTutor { get; set; } = string.Empty;
        public string Materia { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Hora { get; set; } = string.Empty;
    }
}
