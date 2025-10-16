namespace EduConnect_API.Dtos
{
    public class CrearMateriaDto
    {
        public string CodMateria { get; set; } = string.Empty;
        public string NomMateria { get; set; } = string.Empty;
        public int NumCreditos { get; set; }
        public string DescripMateria { get; set; } = string.Empty;
        public int IdSemestre { get; set; }
        public int IdCarrera { get; set; }
    }
}
