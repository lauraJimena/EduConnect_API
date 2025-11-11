namespace EduConnect_API.Dtos
{
    public class MateriaDto
    {
        public int IdMateria { get; set; }
        public string NombreMateria { get; set; } = string.Empty;
        public string CarreraNombre { get; set; } = string.Empty;
        public int Semestre { get; set; }
        public bool TieneMateria { get; set; }
    }
}
