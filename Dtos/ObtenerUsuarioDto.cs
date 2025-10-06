namespace EduConnect_API.Dtos
{
    public class ObtenerUsuarioDto
    {
        public int IdUsu { get; set; }
        public string Nombre { get; set; }=string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string TelUsu { get; set; } = string.Empty;
        public string NumIdent { get; set; } = string.Empty;
        public string TipoIdent { get; set; } = string.Empty;
        public string ContrasUsu { get; set; } = string.Empty;
        public string Carrera { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int IdSemestre { get; set; }                                        
        public int IdRol { get; set; }
        public int IdEstado { get; set; }
        public int IdTipoIdent { get; set; }    
        public int IdCarrera { get; set; }  
        //public string Carrera { get; set; } = string.Empty; 


    }
}
