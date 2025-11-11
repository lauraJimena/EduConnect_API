namespace EduConnect_API.Dtos
{
    public class ReporteDemandaAcademicaDto
    {
        // 🔹 Top 5 materias con más tutorías
        public List<ItemConteoDto> TopMaterias { get; set; } = new();

        // 🔹 Top 5 carreras con más tutorías
        public List<ItemConteoDto> TopCarreras { get; set; } = new();

        // 🔹 Distribución de tutorías por semestre
        public List<ItemConteoDto> TutoríasPorSemestre { get; set; } = new();

        // 🔹 Horarios más solicitados
        public List<ItemConteoDto> HorariosPopulares { get; set; } = new();
        public class ItemConteoDto
        {
            public string Nombre { get; set; } = string.Empty;
            public int Cantidad { get; set; }
        }
    }
}
