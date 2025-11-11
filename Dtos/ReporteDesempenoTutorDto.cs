namespace EduConnect_API.Dtos
{
    public class ReporteDesempenoTutorDto
    {
        public string NombreTutor { get; set; } = string.Empty;
        public string CorreoTutor { get; set; } = string.Empty;
        public int TotalSolicitudes { get; set; }
        public int Aceptadas { get; set; }
        public int Finalizadas { get; set; }
        public int Rechazadas { get; set; }
    }
}
