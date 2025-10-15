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

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
