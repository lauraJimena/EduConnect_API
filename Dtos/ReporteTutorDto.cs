namespace EduConnect_API.Dtos
{
    public class ReporteTutorDto
    {
        public int IdTutor { get; set; }
        public string NombreTutor { get; set; } = string.Empty;
        public string Carrera { get; set; } = string.Empty;
        public int Semestre { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int CantidadMaterias { get; set; }
        public double PromedioCalificacion { get; set; }
    }
}
