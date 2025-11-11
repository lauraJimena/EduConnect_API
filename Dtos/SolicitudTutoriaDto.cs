namespace EduConnect_API.Dtos
{
    public class SolicitudTutoriaDto
    {
    public int IdTutoria { get; set; }
    public string NombreTutor { get; set; } = string.Empty;
    public string MateriaSolicitada { get; set; } = string.Empty;
    public DateTime FechaPropuesta { get; set; }
    public TimeSpan HoraPropuesta { get; set; }
    public string TemaRequerido { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public int IdEstado { get; set; }
    
    }
}
