namespace EduConnect_API.Dtos
{
    public class MateriasDto
    {
        public int IdMateria { get; set; }
        public string CodMateria { get; set; } = string.Empty;
        public string NomMateria { get; set; } = string.Empty;
        public int NumCreditos { get; set; }
        public string DescripMateria { get; set; } = string.Empty;
        public int IdEstado { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int IdSemestre { get; set; }
        public string Semestre { get; set; } = string.Empty;
        public int IdCarrera { get; set; }
        public string Carrera { get; set; } = string.Empty;
    }
}
