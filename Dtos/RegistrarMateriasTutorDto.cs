namespace EduConnect_API.Dtos
{
    public class RegistrarMateriasTutorDto
    {
        public int IdTutor { get; set; }
        public List<int> Materias { get; set; } = new();
    }
}
