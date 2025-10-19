namespace EduConnect_API.Dtos
{
    public class PerfilTutorDto
    {
        public int IdUsuario { get; set; }
        public string NombreTutor { get; set; }
        public string Carrera { get; set; }
        public string Semestre { get; set; }
        public string Materias { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string? AvatarUrl { get; set; } // Ruta o nombre de imagen
    }
}
