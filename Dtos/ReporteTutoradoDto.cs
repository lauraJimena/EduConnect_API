namespace EduConnect_API.Dtos
{
    public class ReporteTutoradoDto
    {
        public int IdUsuario { get; set; }
        public string NombreTutorado { get; set; } = string.Empty;
        public string Carrera { get; set; } = string.Empty;
        public int TotalTutorias { get; set; }
        public string MateriasMasSolicitadas { get; set; } = string.Empty;
        public DateTime? UltimaTutoria { get; set; }
    }
}

