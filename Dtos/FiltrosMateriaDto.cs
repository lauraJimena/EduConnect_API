namespace EduConnect_API.Dtos
{
    public class FiltrosMateriaDto
    {
        public string? MateriaNombre { get; set; }
        public string? Semestre { get; set; }        
        public string? CarreraNombre { get; set; }

        // Paginación
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
