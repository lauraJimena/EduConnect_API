namespace EduConnect_API.Dtos
{
    public class BuscarTutorDto
    {
        public string? Nombre { get; set; }

        // Filtros específicos
        public string? MateriaNombre { get; set; }
        public string? Semestre { get; set; }
        public string? CarreraNombre { get; set; }

        // Estado del tutor
        public int? IdEstado { get; set; }
    }
}
