namespace EduConnect_API.Dtos
{
    public class ObtenerMateriaDto
    {
        public int IdMateria { get; set; }
        public string MateriaNombre { get; set; } = string.Empty;
        public string CarreraNombre { get; set; } = string.Empty;
        public string Semestre { get; set; } = string.Empty;
    }
}
