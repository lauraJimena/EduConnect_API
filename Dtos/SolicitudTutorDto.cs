namespace EduConnect_API.Dtos
{
    public class SolicitudTutorDto
    {
        public int IdTutoria { get; set; }
        public string NombreTutorado { get; set; } = string.Empty;
        public string Materia { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Hora { get; set; } = string.Empty;
        public string Tema { get; set; } = string.Empty;
        public int IdEstado { get; set; }
        //public int IdModalidad { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Modalidad { get; set; } = string.Empty;
        public int IdModalidad { get; set; }
    }
}
