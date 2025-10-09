namespace EduConnect_API.Dtos
{
    public class DetallesTutoriaDto
    {
        public string NombreTutorado { get; set; } = string.Empty;
        public DateTime FechaSugerida { get; set; }
        public TimeSpan HoraSugerida { get; set; }
        public string Materia { get; set; } = string.Empty;
        public string TemaRequerido { get; set; } = string.Empty;
        public string Modalidad { get; set; } = string.Empty;
        public string? ComentarioAdicional { get; set; }
    }
}
