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
    c.nom_carrera                          AS Carrera,
    m.id_materia                           AS IdMateria
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
                    CarreraNombre = reader.GetString(reader.GetOrdinal("Carrera")),
                    IdMateria = reader.GetInt32(reader.GetOrdinal("IdMateria"))
                });
            }

            return lista;
        }
        public async Task<int> ActualizarPerfilTutor(EditarPerfilDto tutor)
        {
            const string sql = @"
UPDATE [EduConnect].[dbo].[usuario]
SET nom_usu = @nom_usu,
    apel_usu = @apel_usu,
    id_tipo_ident = @id_tipo_ident,
    num_ident = @num_ident,
    correo_usu = @correo_usu,
    tel_usu = @tel_usu
WHERE id_usu = @id_usu";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@id_usu", tutor.IdUsu);
                command.Parameters.AddWithValue("@nom_usu", tutor.Nombre);
                command.Parameters.AddWithValue("@apel_usu", tutor.Apellido);
                command.Parameters.AddWithValue("@id_tipo_ident", tutor.IdTipoIdent);
                command.Parameters.AddWithValue("@num_ident", tutor.NumIdent);
                command.Parameters.AddWithValue("@correo_usu", tutor.Correo);
                command.Parameters.AddWithValue("@tel_usu", tutor.TelUsu);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el perfil del tutor: " + ex.Message);
            }
        }

        public async Task<bool> ExisteUsuario(int idTutor)
        {
            const string sql = "SELECT COUNT(1) FROM [EduConnect].[dbo].[usuario] WHERE id_usu = @id_usu";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_usu", idTutor);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }
        public async Task<int> ObtenerRolUsuario(int idUsuario)
        {
            const string sql = "SELECT id_rol FROM [EduConnect].[dbo].[usuario] WHERE id_usu = @id_usu";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_usu", idUsuario);

            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }
        // SOLO solicitudes PENDIENTES (id_estado = 4)
        public async Task<IEnumerable<SolicitudTutorDto>> ObtenerSolicitudesTutor(FiltroSolicitudesTutorDto filtro)
{
    var sql = @"
        SELECT 
            t.id_tutoria AS IdTutoria,
            CONCAT(u.nom_usu, ' ', u.apel_usu) AS NombreTutorado,
            m.nom_materia AS Materia,
            t.fecha AS Fecha,
            CONVERT(VARCHAR(5), t.hora, 108) AS Hora,
            t.tema AS Tema,
            t.id_estado AS IdEstado,
            e.nom_estado AS Estado,
            mo.nom_modalidad AS Modalidad,
            mo.id_modalidad AS IdModalidad
        FROM [EduConnect].[dbo].[tutoria] t
        INNER JOIN [EduConnect].[dbo].[usuario] u ON t.id_tutorado = u.id_usu
        INNER JOIN [EduConnect].[dbo].[materia] m ON t.id_materia = m.id_materia
        INNER JOIN [EduConnect].[dbo].[estado] e ON t.id_estado = e.id_estado
        INNER JOIN [EduConnect].[dbo].[modalidad] mo ON t.id_modalidad = mo.id_modalidad
        WHERE t.id_tutor = @IdTutor AND t.id_estado = 4"; // SOLO solicitudes pendientes

    // ✅ Aplicar filtros solo si tienen valor real
    if (filtro.IdMateria.HasValue && filtro.IdMateria.Value > 0)
    {
        sql += " AND t.id_materia = @IdMateria";
    }

    if (filtro.IdModalidad.HasValue && filtro.IdModalidad.Value > 0)
    {
        sql += " AND t.id_modalidad = @IdModalidad";
    }

    sql += " ORDER BY t.fecha DESC, t.hora DESC";

    var lista = new List<SolicitudTutorDto>();

    using var connection = _dbContextUtility.GetOpenConnection();
    using var command = new SqlCommand(sql, connection);

    // Parámetro obligatorio del tutor
    command.Parameters.AddWithValue("@IdTutor", filtro.IdTutor);

    // Parámetros opcionales
    if (filtro.IdMateria.HasValue && filtro.IdMateria.Value > 0)
    {
        command.Parameters.AddWithValue("@IdMateria", filtro.IdMateria.Value);
    }

    if (filtro.IdModalidad.HasValue && filtro.IdModalidad.Value > 0)
    {
        command.Parameters.AddWithValue("@IdModalidad", filtro.IdModalidad.Value);
    }

    using var reader = await command.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        var dto = new SolicitudTutorDto
        {
            IdTutoria = reader.GetInt32(reader.GetOrdinal("IdTutoria")),
            NombreTutorado = reader.GetString(reader.GetOrdinal("NombreTutorado")),
            Materia = reader.GetString(reader.GetOrdinal("Materia")),
            Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
            Hora = reader.GetString(reader.GetOrdinal("Hora")),
            Tema = reader.GetString(reader.GetOrdinal("Tema")),
            IdEstado = Convert.ToInt32(reader["IdEstado"]),
            Estado = reader.GetString(reader.GetOrdinal("Estado")),
            // Si quieres mostrar la modalidad en el front
            Modalidad = reader.GetString(reader.GetOrdinal("Modalidad")),
            IdModalidad = reader.GetInt32(reader.GetOrdinal("IdTutoria"))
        };
        lista.Add(dto);
    }

    return lista;
}



        // ACEPTAR solicitud - cambia estado a 3
        public async Task<int> AceptarSolicitudTutoria(int idTutoria)
        {
            const string sql = @"
UPDATE [EduConnect].[dbo].[tutoria]
SET id_estado = 3
WHERE id_tutoria = @id_tutoria AND id_estado = 4"; // Solo actualiza si está pendiente

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_tutoria", idTutoria);

            return await command.ExecuteNonQueryAsync();
        }

        // RECHAZAR solicitud - cambia estado a 5
        public async Task<int> RechazarSolicitudTutoria(int idTutoria)
        {
            const string sql = @"
UPDATE [EduConnect].[dbo].[tutoria]
SET id_estado = 5
WHERE id_tutoria = @id_tutoria AND id_estado = 4"; // Solo actualiza si está pendiente

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_tutoria", idTutoria);

            return await command.ExecuteNonQueryAsync();
        }

        // DETALLE de solicitud - SOLO con id_tutoria
        public async Task<DetalleSolicitudTutoriaDto> ObtenerDetalleSolicitud(int idTutoria)
        {
            const string sql = @"
SELECT 
    CONCAT(u.nom_usu, ' ', u.apel_usu) AS NombreTutorado,
    t.fecha AS Fecha,
    CONVERT(VARCHAR(5), t.hora, 108) AS Hora,
    m.nom_materia AS Materia,
    t.tema AS Tema,
    mo.nom_modalidad AS Modalidad,
    ISNULL(t.comentario_adic, '') AS ComentarioAdicional,
    t.id_estado AS IdEstado,
    e.nom_estado AS Estado
FROM [EduConnect].[dbo].[tutoria] t
INNER JOIN [EduConnect].[dbo].[usuario] u ON t.id_tutorado = u.id_usu
INNER JOIN [EduConnect].[dbo].[materia] m ON t.id_materia = m.id_materia
INNER JOIN [EduConnect].[dbo].[modalidad] mo ON t.id_modalidad = mo.id_modalidad
INNER JOIN [EduConnect].[dbo].[estado] e ON t.id_estado = e.id_estado
WHERE t.id_tutoria = @id_tutoria";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_tutoria", idTutoria);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new DetalleSolicitudTutoriaDto
                {
                    NombreTutorado = reader.GetString(reader.GetOrdinal("NombreTutorado")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                    Hora = reader.GetString(reader.GetOrdinal("Hora")),
                    Materia = reader.GetString(reader.GetOrdinal("Materia")),
                    Tema = reader.GetString(reader.GetOrdinal("Tema")),
                    Modalidad = reader.GetString(reader.GetOrdinal("Modalidad")),
                    ComentarioAdicional = reader.GetString(reader.GetOrdinal("ComentarioAdicional")),
                    IdEstado = Convert.ToInt32(reader["IdEstado"]),
                    Estado = reader.GetString(reader.GetOrdinal("Estado"))
                };
            }

            throw new Exception("Solicitud no encontrada");
        }

        // Método para obtener materias del tutor (para filtros)
        public async Task<IEnumerable<MateriaDto>> ObtenerMateriasTutor(int idTutor)
        {
            const string sql = @"
SELECT DISTINCT
    m.id_materia AS IdMateria,
    m.nom_materia AS NombreMateria
FROM [EduConnect].[dbo].[tutoria] t
INNER JOIN [EduConnect].[dbo].[materia] m ON t.id_materia = m.id_materia
WHERE t.id_tutor = @id_tutor
ORDER BY m.nom_materia";

            var lista = new List<MateriaDto>();

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_tutor", idTutor);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new MateriaDto
                {
                    IdMateria = reader.GetInt32(reader.GetOrdinal("IdMateria")),
                    NombreMateria = reader.GetString(reader.GetOrdinal("NombreMateria"))
                });
            }

            return lista;
        }
    }
}