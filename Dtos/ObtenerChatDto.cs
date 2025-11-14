namespace EduConnect_API.Dtos
{
    public class ObtenerChatDto
    {
        public int IdChat { get; set; }
        public int IdTutoria { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string NombreReceptor  { get; set; } = string.Empty;
        public string NombreMateria { get; set; } = string.Empty;
        public bool TieneMensajesNoLeidos { get; set; }
    }
}
