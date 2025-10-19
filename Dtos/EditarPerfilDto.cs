namespace EduConnect_API.Dtos
{
    public class EditarPerfilDto
    {
        public int IdUsu { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public int IdTipoIdent { get; set; }
        public string NumIdent { get; set; } = string.Empty;
        public string TelUsu { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string? Avatar { get; set; }
    }
}
    
