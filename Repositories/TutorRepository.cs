using EduConnect_API.Dtos;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Utilities;
using Microsoft.Data.SqlClient;
using System.Data;
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
            
            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand("dbo.usp_Tutoria_ObtenerDetalleSolicitud", connection);
            command.CommandType = CommandType.StoredProcedure;

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

        public async Task<IEnumerable<ObtenerMateriaDto>> BuscarMateriasAsync(FiltrosMateriaDto filtros)
        {
            var lista = new List<ObtenerMateriaDto>();

            var sql = @"
                SELECT
                    m.id_materia                         AS IdMateria,
                    m.nom_materia                        AS MateriaNombre,
                    c.nom_carrera                        AS CarreraNombre,
                    CAST(s.num_semestre AS nvarchar(5))  AS Semestre
                FROM dbo.materia m
                INNER JOIN dbo.semestre s ON s.id_semestre = m.id_semestre
                INNER JOIN dbo.carrera  c ON c.id_carrera  = m.id_carrera
                WHERE
                    (NULLIF(@MateriaNombre,'') IS NULL OR m.nom_materia LIKE '%' + @MateriaNombre + '%')
                    AND (NULLIF(@Semestre,'')      IS NULL OR CAST(s.num_semestre AS nvarchar(5)) LIKE '%' + @Semestre + '%')
                    AND (NULLIF(@CarreraNombre,'') IS NULL OR c.nom_carrera LIKE '%' + @CarreraNombre + '%')
                ORDER BY c.nom_carrera, s.num_semestre, m.nom_materia
                OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";

            using var connection = _dbContextUtility.GetOpenConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);

            int skip = (filtros.Page - 1) * filtros.PageSize;

            command.Parameters.AddWithValue("@MateriaNombre", (object?)filtros.MateriaNombre ?? DBNull.Value);
            command.Parameters.AddWithValue("@Semestre", (object?)filtros.Semestre ?? DBNull.Value);
            command.Parameters.AddWithValue("@CarreraNombre", (object?)filtros.CarreraNombre ?? DBNull.Value);
            command.Parameters.AddWithValue("@Skip", skip);
            command.Parameters.AddWithValue("@Take", filtros.PageSize);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new ObtenerMateriaDto
                {
                    IdMateria = reader.GetInt32(reader.GetOrdinal("IdMateria")),
                    MateriaNombre = reader.GetString(reader.GetOrdinal("MateriaNombre")),
                    CarreraNombre = reader.GetString(reader.GetOrdinal("CarreraNombre")),
                    Semestre = reader.GetString(reader.GetOrdinal("Semestre"))
                });
            }

            return lista;
        }

        public async Task<IEnumerable<ObtenerMateriaDto>> ListarMateriasAsignadasAsync(int idUsuario)
        {
            var lista = new List<ObtenerMateriaDto>();

            var sql = @"
                SELECT
                    m.id_materia                         AS IdMateria,
                    m.nom_materia                        AS MateriaNombre,
                    c.nom_carrera                        AS CarreraNombre,
                    CAST(s.num_semestre AS nvarchar(5))  AS Semestre
                FROM dbo.tutor_materia tm
                INNER JOIN dbo.materia  m ON m.id_materia  = tm.id_materia
                INNER JOIN dbo.semestre s ON s.id_semestre = m.id_semestre
                INNER JOIN dbo.carrera  c ON c.id_carrera  = m.id_carrera
                WHERE tm.id_usu = @IdUsuario
                ORDER BY c.nom_carrera, s.num_semestre, m.nom_materia;";

            using var connection = _dbContextUtility.GetOpenConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@IdUsuario", idUsuario);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new ObtenerMateriaDto
                {
                    IdMateria = reader.GetInt32(reader.GetOrdinal("IdMateria")),
                    MateriaNombre = reader.GetString(reader.GetOrdinal("MateriaNombre")),
                    CarreraNombre = reader.GetString(reader.GetOrdinal("CarreraNombre")),
                    Semestre = reader.GetString(reader.GetOrdinal("Semestre"))
                });
            }

            return lista;
        }

        public async Task<SeleccionarGuardarMateriaResultadoDto> SeleccionarYGuardarAsync(SeleccionarGuardarMateriaDto dto)
        {
            var resultado = new SeleccionarGuardarMateriaResultadoDto();

            // ✅ tomar UN id de la lista
            var idMateria = dto.IdMaterias?.FirstOrDefault() ?? 0;
            if (idMateria <= 0)
                throw new ArgumentException("IdMaterias no puede estar vacío.", nameof(dto.IdMaterias));
            if (!dto.IdCarrera.HasValue)
                throw new ArgumentException("IdCarrera es requerido para insertar.");

            using var connection = _dbContextUtility.GetOpenConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using var tx = connection.BeginTransaction();
            try
            {
                // Insertar
                using (var cmd = new SqlCommand(
                    "INSERT INTO dbo.tutor_materia(id_usu, id_materia, id_carrera) VALUES(@u,@m,@c);",
                    connection, tx))
                {
                    cmd.Parameters.Add("@u", SqlDbType.Int).Value = dto.IdUsuario;
                    cmd.Parameters.Add("@m", SqlDbType.Int).Value = idMateria;               // ✅ int, no List<int>
                    cmd.Parameters.Add("@c", SqlDbType.Int).Value = dto.IdCarrera.Value;
                    await cmd.ExecuteNonQueryAsync();
                }

                // Totales y lista
                int totales;
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM dbo.tutor_materia WHERE id_usu=@u;",
                    connection, tx))
                {
                    cmd.Parameters.Add("@u", SqlDbType.Int).Value = dto.IdUsuario;
                    totales = (int)(await cmd.ExecuteScalarAsync() ?? 0);
                }

                resultado.Insertada = true;
                resultado.Mensaje = "Materia agregada correctamente.";
                resultado.Totales = totales;
                resultado.Restantes = Math.Max(0, 5 - totales);
                resultado.MateriasAsignadas =
                    await ListarMateriasAsignadasInterno(dto.IdUsuario, connection, tx);

                tx.Commit();
                return resultado;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
        public async Task<bool> ExisteMateria(int idMateria)
        {
            using var cn = _dbContextUtility.GetOpenConnection();
            using var cmd = new SqlCommand("SELECT 1 FROM dbo.materia WHERE id_materia=@id;", cn);
            cmd.Parameters.AddWithValue("@id", idMateria);
            var obj = await cmd.ExecuteScalarAsync();
            return obj != null;
        }

        public async Task<int> ObtenerCarreraPorMateria(int idMateria)
        {
            using var cn = _dbContextUtility.GetOpenConnection();
            using var cmd = new SqlCommand("SELECT id_carrera FROM dbo.materia WHERE id_materia=@id;", cn);
            cmd.Parameters.AddWithValue("@id", idMateria);
            var obj = await cmd.ExecuteScalarAsync();
            if (obj == null) throw new InvalidOperationException("La materia no existe.");
            return Convert.ToInt32(obj);
        }

        public async Task<int> ContarMateriasAsignadas(int idUsuario)
        {
            using var cn = _dbContextUtility.GetOpenConnection();
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.tutor_materia WHERE id_usu=@u;", cn);
            cmd.Parameters.AddWithValue("@u", idUsuario);
            var obj = await cmd.ExecuteScalarAsync();
            return (int)(obj ?? 0);
        }

        public async Task<bool> ExisteAsignacion(int idUsuario, int idMateria)
        {
            using var cn = _dbContextUtility.GetOpenConnection();
            using var cmd = new SqlCommand(
                "SELECT 1 FROM dbo.tutor_materia WHERE id_usu=@u AND id_materia=@m;", cn);
            cmd.Parameters.AddWithValue("@u", idUsuario);
            cmd.Parameters.AddWithValue("@m", idMateria);
            var obj = await cmd.ExecuteScalarAsync();
            return obj != null;
        }
        public async Task<int?> ObtenerCarreraDeUsuario(int idUsuario)
        {
            using var cn = _dbContextUtility.GetOpenConnection();
            using var cmd = new SqlCommand("SELECT id_carrera FROM dbo.usuario WHERE id_usu = @id;", cn);
            cmd.Parameters.AddWithValue("@id", idUsuario);

            var obj = await cmd.ExecuteScalarAsync();
            if (obj == null || obj == DBNull.Value)
                return null;

            return Convert.ToInt32(obj);
        }


        // Reusar SELECT de asignadas dentro de la misma tx
        private static async Task<IEnumerable<ObtenerMateriaDto>> ListarMateriasAsignadasInterno(
            int idUsuario, SqlConnection connection, SqlTransaction transaction)
        {
            var lista = new List<ObtenerMateriaDto>();

            var sql = @"
                SELECT
                    m.id_materia                        AS IdMateria,
                    m.nom_materia                       AS MateriaNombre,
                    c.nom_carrera                       AS CarreraNombre,
                    CAST(s.num_semestre AS nvarchar(5)) AS Semestre
                FROM dbo.tutor_materia tm
                JOIN dbo.materia  m ON m.id_materia  = tm.id_materia
                JOIN dbo.semestre s ON s.id_semestre = m.id_semestre
                JOIN dbo.carrera  c ON c.id_carrera  = m.id_carrera
                WHERE tm.id_usu = @IdUsuario
                ORDER BY c.nom_carrera, s.num_semestre, m.nom_materia;";

            using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@IdUsuario", idUsuario);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new ObtenerMateriaDto
                {
                    IdMateria = reader.GetInt32(reader.GetOrdinal("IdMateria")),
                    MateriaNombre = reader.GetString(reader.GetOrdinal("MateriaNombre")),
                    CarreraNombre = reader.GetString(reader.GetOrdinal("CarreraNombre")),
                    Semestre = reader.GetString(reader.GetOrdinal("Semestre"))
                });
            }

            return lista;
        }
    }
}