namespace EduConnect_API.Dtos
{
    public class ActualizarMateriaDto
    {
        public int IdMateria { get; set; }
        public string CodMateria { get; set; } = string.Empty;
        public string NomMateria { get; set; } = string.Empty;
        public int NumCreditos { get; set; }
        public string DescripMateria { get; set; } = string.Empty;
        public int IdEstado { get; set; }
        public int IdSemestre { get; set; }
        public int IdCarrera { get; set; }
    }
}
