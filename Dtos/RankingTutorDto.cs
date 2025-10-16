namespace EduConnect_API.Dtos
{
    public class RankingTutorDto
    {
        public string NombreTutor { get; set; } = string.Empty;
        public string Carrera { get; set; } = string.Empty;
        public string Semestre { get; set; } = string.Empty;
        public string Materias { get; set; } = string.Empty;
        public double PromedioCalificacion { get; set; }
    }
}
