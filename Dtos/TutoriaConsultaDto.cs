namespace EduConnect_API.Dtos
{
    public class TutoriaConsultaDto
    {
        public int IdTutoria { get; set; }
        public string NombreTutorado { get; set; } = string.Empty;
        public string NombreTutor { get; set; } = string.Empty;
        public string Materia { get; set; } = string.Empty;
        public string Carrera { get; set; } = string.Empty;
        public byte SemestreMateria { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
