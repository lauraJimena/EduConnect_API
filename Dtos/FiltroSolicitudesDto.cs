namespace EduConnect_API.Dtos
{
    public class FiltroSolicitudesDto
    {
        public int IdTutorado { get; set; }
        public List<int>? Estados { get; set; } // Lista de IDs de estado para filtrar
    }
}
