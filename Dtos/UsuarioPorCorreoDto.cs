namespace EduConnect_API.Dtos
{
    public class UsuarioPorCorreoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;

        public int IdUsu{ get; set; }
    }
}
