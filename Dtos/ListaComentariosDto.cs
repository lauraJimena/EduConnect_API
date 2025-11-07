namespace EduConnect_API.Dtos
{
    public class ListaComentariosDto
    {
        public int IdComentario { get; set; }
        public string Tutorado { get; set; } = string.Empty;
        public string Tutor { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
        public int Calificacion { get; set; }
        public DateTime Fecha { get; set; }
        public int IdEstado { get; set; }
        public string NomEstado { get; set; } = string.Empty;
        public string? AvatarTutorado { get; set; }
    }
}
