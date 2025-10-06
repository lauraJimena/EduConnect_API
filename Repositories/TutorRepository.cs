using EduConnect_API.Dtos;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Utilities;
using Microsoft.Data.SqlClient;
using System.Text;

namespace EduConnect_API.Repositories
{
    public class TutorRepository : ITutorRepository
    {
        private readonly DbContextUtility _dbContextUtility;

        public TutorRepository(DbContextUtility dbContextUtility)
        {
            _dbContextUtility = dbContextUtility ?? throw new ArgumentNullException(nameof(dbContextUtility));
        }
        public async Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialTutorAsync(int idTutor, List<int>? estados)
        {
            if (idTutor <= 0)
                throw new ArgumentException("El ID del tutor es obligatorio y debe ser mayor que 0.");

            var lista = new List<HistorialTutoriaDto>();

            var sqlBuilder = new StringBuilder(@"
        SELECT 
            t.id_tutoria,
            t.fecha,
            t.hora,
            t.id_modalidad,
            t.tema,
            t.comentario_adic,
            t.id_tutorado,
            t.id_tutor,
            t.id_materia,
            t.id_estado,
            mo.nom_modalidad     AS modalidad_nombre,
            ma.nom_materia       AS materia_nombre,
            es.nom_estado        AS estado,
            u.nom_usu            AS tutorado_nombre,
            u.apel_usu           AS tutorado_apellido
        FROM dbo.tutoria t
        LEFT JOIN dbo.modalidad mo ON mo.id_modalidad = t.id_modalidad
        LEFT JOIN dbo.materia   ma ON ma.id_materia   = t.id_materia
        LEFT JOIN dbo.estado    es ON es.id_estado    = t.id_estado
        LEFT JOIN dbo.usuario   u  ON u.id_usu        = t.id_tutorado
        WHERE t.id_tutor = @idTutor
    ");

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand();
            command.Connection = (SqlConnection)connection;

            command.Parameters.AddWithValue("@idTutor", idTutor);

            // Si hay estados, armamos un IN parametrizado: (@e0, @e1, ...)
            if (estados != null && estados.Any())
            {
                var paramNames = new List<string>();
                for (int i = 0; i < estados.Count; i++)
                {
                    var param = "@e" + i;
                    paramNames.Add(param);
                    command.Parameters.AddWithValue(param, estados[i]);
                }

                sqlBuilder.Append(" AND t.id_estado IN (" + string.Join(", ", paramNames) + ")");
            }
            // else: NO agregamos condición por estado -> devuelve todos los estados

            sqlBuilder.Append(" ORDER BY t.fecha DESC, t.hora DESC;");

            command.CommandText = sqlBuilder.ToString();

            // --- DEBUG opcional: ver el SQL final en consola/log (quita en prod)
            // Console.WriteLine(command.CommandText);
            // foreach(SqlParameter p in command.Parameters) Console.WriteLine($"{p.ParameterName} = {p.Value}");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var dto = new HistorialTutoriaDto
                {
                    IdTutoria = reader.GetInt32(0),
                    Fecha = reader.GetDateTime(1),
                    Hora = reader.GetFieldValue<TimeSpan>(2),
                    IdModalidad = reader.GetByte(3),
                    Tema = reader.IsDBNull(4) ? null : reader.GetString(4),
                    ComentarioAdic = reader.IsDBNull(5) ? null : reader.GetString(5),
                    IdTutorado = reader.GetInt32(6),
                    IdTutor = reader.GetInt32(7),
                    IdMateria = reader.GetInt32(8),
                    IdEstado = reader.GetByte(9),
                    ModalidadNombre = reader.IsDBNull(10) ? null : reader.GetString(10),
                    MateriaNombre = reader.IsDBNull(11) ? null : reader.GetString(11),
                    EstadoNombre = reader.IsDBNull(12) ? null : reader.GetString(12),
                    TutorNombreCompleto = $"{(reader.IsDBNull(13) ? "" : reader.GetString(13))} {(reader.IsDBNull(14) ? "" : reader.GetString(14))}".Trim()
                };

                lista.Add(dto);
            }

            return lista;
        }


        public async Task<IEnumerable<ObtenerTutorDto>> ObtenerTutoresAsync(BuscarTutorDto filtros)
        {
            var lista = new List<ObtenerTutorDto>();

            var sql = @"
               SELECT 
    u.id_usu                               AS IdUsuario,
    (u.nom_usu + ' ' + u.apel_usu)         AS TutorNombreCompleto,
    CAST(u.id_estado AS int)               AS Estado,
    m.nom_materia                          AS Materia,
    s.num_semestre                         AS Semestre,
    c.nom_carrera                          AS Carrera
FROM dbo.usuario AS u
INNER JOIN dbo.tutor_materia AS tm ON tm.id_usu     = u.id_usu
INNER JOIN dbo.materia       AS m  ON m.id_materia  = tm.id_materia
INNER JOIN dbo.semestre      AS s  ON s.id_semestre = m.id_semestre
INNER JOIN dbo.carrera       AS c  ON c.id_carrera  = m.id_carrera
WHERE
    u.id_rol = 2
    AND (NULLIF(@Nombre,  N'') IS NULL OR (u.nom_usu + ' ' + u.apel_usu) LIKE N'%' + @Nombre + N'%')
    AND (NULLIF(@Materia, N'') IS NULL OR m.nom_materia  LIKE N'%' + @Materia + N'%')
    AND (@Semestre IS NULL OR s.num_semestre = @Semestre)
    AND (NULLIF(@Carrera, N'') IS NULL OR c.nom_carrera  LIKE N'%' + @Carrera + N'%')
    AND (@Estado   IS NULL OR u.id_estado = @Estado)
ORDER BY u.nom_usu, u.apel_usu, m.nom_materia;";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Nombre", (object?)filtros.Nombre ?? DBNull.Value);
            command.Parameters.AddWithValue("@Materia", (object?)filtros.MateriaNombre ?? DBNull.Value);
            command.Parameters.AddWithValue("@Semestre", (object?)filtros.Semestre ?? DBNull.Value);
            command.Parameters.AddWithValue("@Carrera", (object?)filtros.CarreraNombre ?? DBNull.Value);
            command.Parameters.AddWithValue("@Estado", (object?)filtros.IdEstado ?? DBNull.Value);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new ObtenerTutorDto
                {
                    IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                    TutorNombreCompleto = reader.GetString(reader.GetOrdinal("TutorNombreCompleto")),
                    IdEstado = reader.GetInt32(reader.GetOrdinal("Estado")),
                    MateriaNombre = reader.GetString(reader.GetOrdinal("Materia")),
                    Semestre = reader.GetByte(reader.GetOrdinal("Semestre")),
                    CarreraNombre = reader.GetString(reader.GetOrdinal("Carrera"))
                });
            }

            return lista;
        }

    }
}
