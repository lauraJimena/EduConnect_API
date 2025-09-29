namespace EduConnect_API.Dtos
{
    public class UsuarioRespuesta
    {
        public int IdUsu { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? NumIdent { get; set; }
        public string? Correo { get; set; }
        public byte IdRol { get; set; }
        public byte IdEstado { get; set; }
    }
}
