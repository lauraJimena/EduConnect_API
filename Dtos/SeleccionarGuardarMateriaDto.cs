namespace EduConnect_API.Dtos
{
    public class SeleccionarGuardarMateriaDto
    {
        public int IdUsuario { get; set; }
        public List<int> IdMaterias{ get; set; } = new();
        public int? IdCarrera { get; set; }
    }
}
