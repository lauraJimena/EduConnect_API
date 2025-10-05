namespace EduConnect_API.Dtos
{
    public class CrearMensajeDto
    {
        public int IdChat { get; set; }
        public int IdEmisor { get; set; }
        public string Contenido { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
    }
}
