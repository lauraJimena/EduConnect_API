namespace EduConnect_API.Dtos
{
    public class CrearUsuarioDto
    {
        public string Nombre { get; set; }=string.Empty;
        public string Apellido { get; set; } = string.Empty;    
        public string Correo { get; set; } = string.Empty;

        public int IdTipoIdent { get; set; }
        public string NumIdent { get; set; } = string.Empty;
        public string TelUsu { get; set; } = string.Empty;
        public string ContrasUsu { get; set; } = string.Empty;

        public int IdCarrera { get; set; }
        public int IdSemestre { get; set; }
        public int IdRol { get; set; }
        public int IdEstado { get; set; }
        public bool DebeActualizarPassword { get; set; }

    }
}
