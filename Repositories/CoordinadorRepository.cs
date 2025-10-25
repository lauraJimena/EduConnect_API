using EduConnect_API.Dtos;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Utilities;
using Microsoft.Data.SqlClient;
using static EduConnect_API.Dtos.ReporteDemandaAcademicaDto;

namespace EduConnect_API.Repositories
{
    public class CoordinadorRepository: ICoordinadorRepository
    {
        private readonly DbContextUtility _dbContextUtility;

        public CoordinadorRepository(DbContextUtility dbContextUtility)
        {
            _dbContextUtility = dbContextUtility ?? throw new ArgumentNullException(nameof(dbContextUtility));
        }
        public async Task<IEnumerable<TutoriaConsultaDto>> ConsultarTutoriasAsync(
     string? carrera, int? semestre, string? materia, int? idEstado, int? ordenFecha)
        {
            const string sql = @"
    SELECT 
        t.id_tutoria,
        tu.nom_usu AS NombreTutorado,
        tt.nom_usu AS NombreTutor,
        m.nom_materia AS Materia,
        c.nom_carrera AS Carrera,
        m.id_semestre AS SemestreMateria,
        t.fecha,
        CONVERT(VARCHAR(5), t.hora, 108) AS Hora,
        e.nom_estado AS Estado
    FROM tutoria t
    JOIN usuario tu ON t.id_tutorado = tu.id_usu
    JOIN usuario tt ON t.id_tutor = tt.id_usu
    JOIN materia m ON t.id_materia = m.id_materia
    JOIN carrera c ON m.id_carrera = c.id_carrera
    JOIN estado e ON t.id_estado = e.id_estado
    WHERE 
        (@carrera IS NULL OR c.nom_carrera LIKE '%' + @carrera + '%')
        AND (@semestre IS NULL OR m.id_semestre = @semestre)
        AND (@materia IS NULL OR m.nom_materia LIKE '%' + @materia + '%')
        AND (@idEstado IS NULL OR t.id_estado = @idEstado)
    ORDER BY 
        CASE WHEN @ordenFecha = 1 THEN t.fecha END ASC,
        CASE WHEN @ordenFecha = 2 THEN t.fecha END DESC,
        t.hora ASC;";

            var lista = new List<TutoriaConsultaDto>();

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@carrera", (object?)carrera ?? DBNull.Value);
                command.Parameters.AddWithValue("@semestre", (object?)semestre ?? DBNull.Value);
                command.Parameters.AddWithValue("@materia", (object?)materia ?? DBNull.Value);
                command.Parameters.AddWithValue("@idEstado", (object?)idEstado ?? DBNull.Value);
                command.Parameters.AddWithValue("@ordenFecha", (object?)ordenFecha ?? DBNull.Value);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    lista.Add(new TutoriaConsultaDto
                    {
                        IdTutoria = reader.GetInt32(0),
                        NombreTutorado = reader.GetString(1),
                        NombreTutor = reader.GetString(2),
                        Materia = reader.GetString(3),
                        Carrera = reader.GetString(4),
                        SemestreMateria = reader.GetByte(5),
                        Fecha = reader.GetDateTime(6),
                        Hora = reader.GetString(7),
                        Estado = reader.GetString(8)
                    });
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar tutorías: " + ex.Message);
            }
        }
        public async Task<ReporteDemandaAcademicaDto> ObtenerReporteDemandaAcademicaAsync()
        {
            var query = @"
SELECT TOP 5 m.nom_materia AS Nombre, COUNT(*) AS Cantidad
FROM tutoria t
JOIN materia m ON t.id_materia = m.id_materia
GROUP BY m.nom_materia
ORDER BY COUNT(*) DESC;

-- 🔸 Top 5 carreras con más tutorías
SELECT TOP 5 c.nom_carrera AS Nombre, COUNT(*) AS Cantidad
FROM tutoria t
JOIN materia m ON t.id_materia = m.id_materia
JOIN carrera c ON m.id_carrera = c.id_carrera
GROUP BY c.nom_carrera
ORDER BY COUNT(*) DESC;

-- 🔸 Distribución por semestre
SELECT m.id_semestre AS Nombre, COUNT(*) AS Cantidad
FROM tutoria t
JOIN materia m ON t.id_materia = m.id_materia
GROUP BY m.id_semestre
ORDER BY m.id_semestre;

-- 🔸 Horarios más solicitados
SELECT  CONVERT(VARCHAR(5), t.hora, 108) AS Nombre, COUNT(*) AS Cantidad
FROM tutoria t
GROUP BY  CONVERT(VARCHAR(5), t.hora, 108)
ORDER BY COUNT(*) DESC;
";

            var resultado = new ReporteDemandaAcademicaDto();

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            // 🔹 1. Materias
            while (await reader.ReadAsync())
                resultado.TopMaterias.Add(new ItemConteoDto { Nombre = reader["Nombre"].ToString(), Cantidad = Convert.ToInt32(reader["Cantidad"]) });

            // 🔹 2. Carreras
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
                resultado.TopCarreras.Add(new ItemConteoDto { Nombre = reader["Nombre"].ToString(), Cantidad = Convert.ToInt32(reader["Cantidad"]) });

            // 🔹 3. Semestres
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
                resultado.TutoríasPorSemestre.Add(new ItemConteoDto { Nombre = reader["Nombre"].ToString(), Cantidad = Convert.ToInt32(reader["Cantidad"]) });

            // 🔹 4. Horarios
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
                resultado.HorariosPopulares.Add(new ItemConteoDto { Nombre = reader["Nombre"].ToString(), Cantidad = Convert.ToInt32(reader["Cantidad"]) });

            return resultado;
        }
        public async Task<(ReporteGestionAdministrativaDto Totales, List<ReporteDesempenoTutorDto> Desempeno)> 
    ObtenerReporteCombinadoAsync()
{
    const string sql = @"
        SELECT 
            (SELECT COUNT(*) FROM usuario WHERE id_rol = 1 AND id_estado = 1) AS TotalTutoresActivos,
            (SELECT COUNT(*) FROM usuario WHERE id_rol = 2 AND id_estado = 1) AS TotalTutoradosActivos,
            (SELECT COUNT(*) FROM tutoria) AS TotalTutorias,
            (SELECT COUNT(*) FROM tutoria WHERE id_modalidad = 2) AS Virtuales,
            (SELECT COUNT(*) FROM tutoria WHERE id_modalidad = 1) AS Presenciales;

        SELECT 
            u.nom_usu AS NombreTutor,
            u.correo_usu AS CorreoTutor,
            COUNT(t.id_tutoria) AS TotalSolicitudes,
            SUM(CASE WHEN t.id_estado = 3 THEN 1 ELSE 0 END) AS Aceptadas,
            SUM(CASE WHEN t.id_estado = 6 THEN 1 ELSE 0 END) AS Finalizadas,
            SUM(CASE WHEN t.id_estado = 5 THEN 1 ELSE 0 END) AS Rechazadas
        FROM tutoria t
        INNER JOIN usuario u ON t.id_tutor = u.id_usu
        GROUP BY u.nom_usu, u.correo_usu
        ORDER BY TotalSolicitudes DESC;
    ";

    using var connection = _dbContextUtility.GetOpenConnection();
    using var command = new SqlCommand(sql, connection);
    using var reader = await command.ExecuteReaderAsync();

    var totales = new ReporteGestionAdministrativaDto();
    var listaTutores = new List<ReporteDesempenoTutorDto>();

    // Primer SELECT: Totales
    if (await reader.ReadAsync())
    {
        totales.TotalTutoresActivos = reader.GetInt32(0);
        totales.TotalTutoradosActivos = reader.GetInt32(1);
        totales.TotalTutorias = reader.GetInt32(2);
        totales.Virtuales = reader.GetInt32(3);
        totales.Presenciales = reader.GetInt32(4);
    }

    // Segundo SELECT: Desempeño por tutor
    await reader.NextResultAsync();
    while (await reader.ReadAsync())
    {
        listaTutores.Add(new ReporteDesempenoTutorDto
        {
            NombreTutor = reader.GetString(0),
            CorreoTutor = reader.GetString(1),
            TotalSolicitudes = reader.GetInt32(2),
            Aceptadas = reader.GetInt32(3),
            Finalizadas = reader.GetInt32(4),
            Rechazadas = reader.GetInt32(5)
        });
    }

    return (totales, listaTutores);
}



    }
}
