namespace EduConnect_API.Dtos
{
    public class SeleccionarGuardarMateriaResultadoDto
    {
        public bool Insertada { get; set; }
        public int Totales { get; set; }
        public int Restantes { get; set; }
        public IEnumerable<ObtenerMateriaDto> MateriasAsignadas { get; set; } = Enumerable.Empty<ObtenerMateriaDto>();
        public string Mensaje { get; set; } = string.Empty;
    }
}
