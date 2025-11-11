namespace EduConnect_API.Dtos
{
    public class HistorialTutoriaDto
    {
        // --- Datos base de la tutoría ---
        public int IdTutoria { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }

        public string? Tema { get; set; }
        public string? ComentarioAdic { get; set; }

        // --- Llaves foráneas ---
        public byte IdModalidad { get; set; }
        public int IdTutorado { get; set; }
        public int IdTutor { get; set; }
        public int IdMateria { get; set; }
        public byte IdEstado { get; set; }

        // --- Campos enriquecidos (de joins) ---
        public string? ModalidadNombre { get; set; }
        public string? MateriaNombre { get; set; }
        public string? EstadoNombre { get; set; }

        // Tutor (nombre + apellido unidos en el repo)
        public string? TutorNombreCompleto { get; set; }
    }
}
