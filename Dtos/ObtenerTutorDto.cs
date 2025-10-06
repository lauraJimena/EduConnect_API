namespace EduConnect_API.Dtos
{
    public class ObtenerTutorDto
    {
        public int IdUsuario { get; set; }
        public string TutorNombreCompleto { get; set; } = string.Empty;
        public int IdEstado { get; set; }
        public string MateriaNombre { get; set; } = string.Empty;
        public byte Semestre { get; set; }
        public string CarreraNombre { get; set; } = string.Empty;
    }
}
