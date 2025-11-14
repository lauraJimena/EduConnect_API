namespace EduConnect_API.Dtos
{
    public class TokenRecuperacionDto
    {
        public string Correo { get; set; } = string.Empty;
        public DateTime FechaExpira { get; set; }
        public bool Usado { get; set; }
    }

}
