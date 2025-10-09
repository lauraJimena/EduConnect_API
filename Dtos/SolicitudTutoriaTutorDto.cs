namespace EduConnect_API.Dtos
{
    public class SolicitudTutoriaTutorDto
    {
        public int IdTutoria { get; set; }
        public string NombreMateria { get; set; } = string.Empty;
        public DateTime FechaSugerida { get; set; }
        public TimeSpan HoraSugerida { get; set; }
        public string NombreTutorado { get; set; } = string.Empty;
    }
}
