namespace EduConnect_API.Dtos
{
    public class ReporteGestionAdministrativaDto
    {
        public int TotalTutoresActivos { get; set; }
        public int TotalTutoradosActivos { get; set; }
        public int TotalTutorias { get; set; }
        public int Virtuales { get; set; }
        public int Presenciales { get; set; }

        public double PorcentajeVirtual => TotalTutorias > 0 ? Math.Round((double)Virtuales / TotalTutorias * 100, 2) : 0;
        public double PorcentajePresencial => TotalTutorias > 0 ? Math.Round((double)Presenciales / TotalTutorias * 100, 2) : 0;
    }
}
