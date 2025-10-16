using EduConnect_API.Dtos;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Utilities;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace EduConnect_API.Repositories
{
    public class TutoradoRepository : ITutoradoRepository
    {
        private readonly DbContextUtility _dbContextUtility;

        public TutoradoRepository(DbContextUtility dbContextUtility)
        {
            _dbContextUtility = dbContextUtility ?? throw new ArgumentNullException(nameof(dbContextUtility));
        }

        public async Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialTutoradoAsync(int idTutorado, List<int>? idsEstado)
        {
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

                    -- Textos relacionados
                    mo.nom_modalidad AS modalidad_nombre,
                    ma.nom_materia AS materia_nombre,
                    es.nom_estado AS estado,
                    u.nom_usu AS tutor_nombre,
                    u.apel_usu AS tutor_apellido
                FROM [EduConnect].[dbo].[tutoria] AS t
                LEFT JOIN [EduConnect].[dbo].[modalidad] AS mo ON mo.id_modalidad = t.id_modalidad
                LEFT JOIN [EduConnect].[dbo].[materia] AS ma ON ma.id_materia = t.id_materia
                LEFT JOIN [EduConnect].[dbo].[estado] AS es ON es.id_estado = t.id_estado
                LEFT JOIN [EduConnect].[dbo].[usuario] AS u ON u.id_usu = t.id_tutor
                WHERE t.id_tutorado = @idTutorado AND t.id_estado IN (3, 6, 7)
            ");

            // ✅ Solo agrega el filtro si hay estados en la lista
            if (idsEstado != null && idsEstado.Count > 0)
            {
                sqlBuilder.Append(" AND t.id_estado IN (");
                for (int i = 0; i < idsEstado.Count; i++)
                {
                    sqlBuilder.Append($"@idEstado{i}");
                    if (i < idsEstado.Count - 1)
                        sqlBuilder.Append(", ");
                }
                sqlBuilder.Append(")");
            }

            sqlBuilder.Append(" ORDER BY t.fecha DESC, t.hora DESC");

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sqlBuilder.ToString(), connection);

            // Parámetro obligatorio (idTutorado)
            command.Parameters.AddWithValue("@idTutorado", idTutorado);

            // Parámetros de estados si aplica
            if (idsEstado != null && idsEstado.Count > 0)
            {
                for (int i = 0; i < idsEstado.Count; i++)
                    command.Parameters.AddWithValue($"@idEstado{i}", idsEstado[i]);
            }

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
        public async Task<int> ActualizarPerfil(EditarPerfilDto perfil)
        {

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand("dbo.usp_Usuario_ActualizarPerfil", connection);

                command.Parameters.AddWithValue("@id_usu", perfil.IdUsu);
                command.Parameters.AddWithValue("@nom_usu", perfil.Nombre);
                command.Parameters.AddWithValue("@apel_usu", perfil.Apellido);
                command.Parameters.AddWithValue("@id_tipo_ident", perfil.IdTipoIdent);
                command.Parameters.AddWithValue("@num_ident", perfil.NumIdent);
                command.Parameters.AddWithValue("@correo_usu", perfil.Correo);
                command.Parameters.AddWithValue("@tel_usu", perfil.TelUsu);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el perfil del tutorado: " + ex.Message);
            }
        }

        public async Task<bool> ExisteUsuario(int idUsuario)
        {
            const string sql = "SELECT COUNT(1) FROM [EduConnect].[dbo].[usuario] WHERE id_usu = @id_usu";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_usu", idUsuario);

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
        public async Task<IEnumerable<ObtenerTutorDto>> ObtenerTutoresAsync(BuscarTutorDto filtros)
        {
            var lista = new List<ObtenerTutorDto>();

                

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand("dbo.usp_Tutores_ListarMaterias", connection);
            command.CommandType = CommandType.StoredProcedure;

            int skip = (filtros.Page - 1) * filtros.PageSize;
            command.Parameters.AddWithValue("@Skip", skip);
            command.Parameters.AddWithValue("@Take", filtros.PageSize);

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
        public async Task<IEnumerable<SolicitudTutoriaDto>> ObtenerSolicitudesTutorias(FiltroSolicitudesDto filtro)
        {
            var sql = @"
SELECT 
    t.id_tutoria AS IdTutoria,
    CONCAT(u.nom_usu, ' ', u.apel_usu) AS NombreTutor,
    m.nom_materia AS MateriaSolicitada,
    t.fecha AS FechaPropuesta,
    t.hora AS HoraPropuesta,
    t.tema AS TemaRequerido,
    e.nom_estado AS Estado,
    t.id_estado AS IdEstado
FROM [EduConnect].[dbo].[tutoria] t
INNER JOIN [EduConnect].[dbo].[usuario] u ON t.id_tutor = u.id_usu
INNER JOIN [EduConnect].[dbo].[materia] m ON t.id_materia = m.id_materia
INNER JOIN [EduConnect].[dbo].[estado] e ON t.id_estado = e.id_estado
WHERE t.id_tutorado = @IdTutorado AND t.id_estado IN (3, 4, 5)";

            // Agregar filtro por estados si se especifican
            if (filtro.Estados != null && filtro.Estados.Count > 0)
            {
                sql += " AND t.id_estado IN (";
                for (int i = 0; i < filtro.Estados.Count; i++)
                {
                    sql += $"@Estado{i}";
                    if (i < filtro.Estados.Count - 1)
                        sql += ", ";
                }
                sql += ")";
            }

            sql += " ORDER BY t.fecha DESC, t.hora DESC";

            var lista = new List<SolicitudTutoriaDto>();

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@IdTutorado", filtro.IdTutorado);

            // Agregar parámetros para los estados si existen
            if (filtro.Estados != null && filtro.Estados.Count > 0)
            {
                for (int i = 0; i < filtro.Estados.Count; i++)
                {
                    command.Parameters.AddWithValue($"@Estado{i}", filtro.Estados[i]);
                }
            }

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var dto = new SolicitudTutoriaDto
                {
                    IdTutoria = reader.GetInt32(reader.GetOrdinal("IdTutoria")),
                    NombreTutor = reader.GetString(reader.GetOrdinal("NombreTutor")),
                    MateriaSolicitada = reader.GetString(reader.GetOrdinal("MateriaSolicitada")),
                    FechaPropuesta = reader.GetDateTime(reader.GetOrdinal("FechaPropuesta")),
                    HoraPropuesta = reader.GetTimeSpan(reader.GetOrdinal("HoraPropuesta")),
                    TemaRequerido = reader.GetString(reader.GetOrdinal("TemaRequerido")),
                    Estado = reader.GetString(reader.GetOrdinal("Estado")),
                    IdEstado = Convert.ToInt32(reader["IdEstado"]) // CORREGIDO: Conversión segura
                };
                lista.Add(dto);
            }

            return lista;
        }

        public async Task<IEnumerable<EstadoSolicitudDto>> ObtenerEstadosSolicitud()
        {
            const string sql = @"
SELECT 
    id_estado AS IdEstado,
    nom_estado AS NombreEstado
FROM [EduConnect].[dbo].[estado]
WHERE id_estado IN (1, 2, 3)";

            var lista = new List<EstadoSolicitudDto>();

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var dto = new EstadoSolicitudDto
                {
                    IdEstado = Convert.ToInt32(reader["IdEstado"]), // CORREGIDO: Conversión segura
                    NombreEstado = reader.GetString(reader.GetOrdinal("NombreEstado"))
                };
                lista.Add(dto);
            }

            return lista;
        }
        /*public async Task<int> CrearSolicitudTutoria(SolicitudTutoriaRequestDto solicitud)
        {
            const string sql = @"
INSERT INTO [EduConnect].[dbo].[tutoria] 
    (fecha, hora, id_modalidad, tema, comentario_adic, id_tutorado, id_tutor, id_materia, id_estado)
VALUES 
    (@fecha, @hora, @id_modalidad, @tema, @comentario_adic, @id_tutorado, @id_tutor, @id_materia, 4)"; // id_estado = 4 (Pendiente)

            try
            {
                // Convertir string a TimeSpan
                if (!TimeSpan.TryParse(solicitud.Hora, out TimeSpan horaTimeSpan))
                {
                    throw new ArgumentException("Formato de hora inválido");
                }

                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@fecha", solicitud.Fecha);
                command.Parameters.AddWithValue("@hora", horaTimeSpan);
                command.Parameters.AddWithValue("@id_modalidad", solicitud.IdModalidad);
                command.Parameters.AddWithValue("@tema", solicitud.Tema);
                command.Parameters.AddWithValue("@comentario_adic", (object)solicitud.ComentarioAdicional ?? DBNull.Value);
                command.Parameters.AddWithValue("@id_tutorado", solicitud.IdTutorado);
                command.Parameters.AddWithValue("@id_tutor", solicitud.IdTutor);
                command.Parameters.AddWithValue("@id_materia", solicitud.IdMateria);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la solicitud de tutoría: " + ex.Message);
            }
        }*/

        public async Task<int> CrearSolicitudTutoria(SolicitudTutoriaRequestDto solicitud)
        {
            const string sql = @"
INSERT INTO [EduConnect].[dbo].[tutoria] 
    (fecha, hora, id_modalidad, tema, comentario_adic, id_tutorado, id_tutor, id_materia, id_estado)
OUTPUT INSERTED.id_tutoria
VALUES 
    (@fecha, @hora, @id_modalidad, @tema, @comentario_adic, @id_tutorado, @id_tutor, @id_materia, 4);"; // id_estado = 4 (Pendiente)

            try
            {
                if (!TimeSpan.TryParse(solicitud.Hora, out TimeSpan horaTimeSpan))
                {
                    throw new ArgumentException("Formato de hora inválido");
                }

                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@fecha", solicitud.Fecha);
                command.Parameters.AddWithValue("@hora", horaTimeSpan);
                command.Parameters.AddWithValue("@id_modalidad", solicitud.IdModalidad);
                command.Parameters.AddWithValue("@tema", solicitud.Tema);
                command.Parameters.AddWithValue("@comentario_adic", (object)solicitud.ComentarioAdicional ?? DBNull.Value);
                command.Parameters.AddWithValue("@id_tutorado", solicitud.IdTutorado);
                command.Parameters.AddWithValue("@id_tutor", solicitud.IdTutor);
                command.Parameters.AddWithValue("@id_materia", solicitud.IdMateria);

                // Aquí recibes el ID generado automáticamente
                var idGenerado = await command.ExecuteScalarAsync();

                return Convert.ToInt32(idGenerado);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la solicitud de tutoría: " + ex.Message);
            }
        }

        public async Task<bool> ExisteTutor(int idTutor)
        {
            const string sql = "SELECT COUNT(1) FROM [EduConnect].[dbo].[usuario] WHERE id_usu = @id_tutor AND id_rol = 2"; // Rol 2 = Tutor
            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_tutor", idTutor);
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> ExisteMateria(int idMateria)
        {
            const string sql = "SELECT COUNT(1) FROM [EduConnect].[dbo].[materia] WHERE id_materia = @id_materia";
            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_materia", idMateria);
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> ExisteModalidad(int idModalidad)
        {
            const string sql = "SELECT COUNT(1) FROM [EduConnect].[dbo].[modalidad] WHERE id_modalidad = @id_modalidad";
            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_modalidad", idModalidad);
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }
        public async Task<ComentarioResponseDto> CrearComentario(CrearComentarioDto comentario)
        {
            const string sql = @"
INSERT INTO [EduConnect].[dbo].[comentario] 
    (texto, calificacion, fecha, id_tutor, id_tutorado, id_estado)
OUTPUT INSERTED.id_comentario, INSERTED.texto, INSERTED.calificacion, INSERTED.fecha, 
       INSERTED.id_tutor, INSERTED.id_tutorado, INSERTED.id_estado
VALUES 
    (@texto, @calificacion, GETDATE(), @id_tutor, @id_tutorado, 1)"; // id_estado = 1 (Activo)

            const string sqlDatos = @"
SELECT 
    c.id_comentario,
    c.texto,
    c.calificacion,
    c.fecha,
    c.id_tutor,
    c.id_tutorado,
    c.id_estado,
    CONCAT(tutor.nom_usu, ' ', tutor.apel_usu) AS NombreTutor,
    CONCAT(tutorado.nom_usu, ' ', tutorado.apel_usu) AS NombreTutorado
FROM [EduConnect].[dbo].[comentario] c
INNER JOIN [EduConnect].[dbo].[usuario] tutor ON c.id_tutor = tutor.id_usu
INNER JOIN [EduConnect].[dbo].[usuario] tutorado ON c.id_tutorado = tutorado.id_usu
WHERE c.id_comentario = @id_comentario";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();

                // Insertar el comentario
                int idComentario;
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@texto", comentario.Texto);
                    command.Parameters.AddWithValue("@calificacion", comentario.Calificacion);
                    command.Parameters.AddWithValue("@id_tutor", comentario.IdTutor);
                    command.Parameters.AddWithValue("@id_tutorado", comentario.IdTutorado);

                    idComentario = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                // Obtener los datos completos del comentario creado
                using var commandDatos = new SqlCommand(sqlDatos, connection);
                commandDatos.Parameters.AddWithValue("@id_comentario", idComentario);

                using var reader = await commandDatos.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new ComentarioResponseDto
                    {
                        IdComentario = reader.GetInt32(reader.GetOrdinal("id_comentario")),
                        Texto = reader.GetString(reader.GetOrdinal("texto")),
                        Calificacion = Convert.ToInt32(reader["calificacion"]),
                        Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                        IdTutor = reader.GetInt32(reader.GetOrdinal("id_tutor")),
                        IdTutorado = reader.GetInt32(reader.GetOrdinal("id_tutorado")),
                        IdEstado = Convert.ToInt32(reader["id_estado"]),
                        NombreTutor = reader.GetString(reader.GetOrdinal("NombreTutor")),
                        NombreTutorado = reader.GetString(reader.GetOrdinal("NombreTutorado"))
                    };
                }

                throw new Exception("Error al recuperar el comentario creado");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el comentario: " + ex.Message);
            }
        }

        public async Task<IEnumerable<RankingTutorDto>> ObtenerRankingTutores()
        {
            // Consulta CORREGIDA usando id_usu en tutor_materia
            const string sql = @"
SELECT 
    CONCAT(u.nom_usu, ' ', u.apel_usu) AS NombreTutor,
    c.nom_carrera AS Carrera,
    s.num_semestre AS Semestre,
    ISNULL((
        SELECT STRING_AGG(m.nom_materia, ', ') 
        FROM [EduConnect].[dbo].[tutor_materia] tm
        INNER JOIN [EduConnect].[dbo].[materia] m ON tm.id_materia = m.id_materia
        WHERE tm.id_usu = u.id_usu  -- CORREGIDO: usar id_usu en lugar de id_tutor
    ), 'Sin materias registradas') AS Materias,
    ISNULL((
        SELECT AVG(CAST(calificacion AS FLOAT))
        FROM [EduConnect].[dbo].[comentario] 
        WHERE id_tutor = u.id_usu AND id_estado = 1  -- id_tutor se refiere al id_usu del tutor
    ), 0) AS PromedioCalificacion
FROM [EduConnect].[dbo].[usuario] u
INNER JOIN [EduConnect].[dbo].[carrera] c ON u.id_carrera = c.id_carrera
INNER JOIN [EduConnect].[dbo].[semestre] s ON u.id_semestre = s.id_semestre
WHERE u.id_rol = 2 -- Solo tutores
    AND u.id_estado = 1 -- Solo usuarios activos
ORDER BY PromedioCalificacion DESC";

            var lista = new List<RankingTutorDto>();

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var dto = new RankingTutorDto
                {
                    NombreTutor = reader.GetString(reader.GetOrdinal("NombreTutor")),
                    Carrera = reader.GetString(reader.GetOrdinal("Carrera")),
                    Semestre = Convert.ToString(reader["Semestre"]),
                    Materias = reader.GetString(reader.GetOrdinal("Materias")),
                    PromedioCalificacion = Convert.ToDouble(reader["PromedioCalificacion"])
                };
                lista.Add(dto);
            }

            return lista;
        }
        public async Task<IEnumerable<ComentarioTutorInfoDto>> ObtenerComentariosPorTutor(int idTutor)
        {
            const string sql = @"
SELECT 
    CONCAT(u.nom_usu, ' ', u.apel_usu) AS Usuario,
    c.texto AS Comentario,
    c.calificacion AS Calificacion,
    c.fecha AS Fecha
FROM [EduConnect].[dbo].[comentario] c
INNER JOIN [EduConnect].[dbo].[usuario] u ON c.id_tutorado = u.id_usu
WHERE c.id_tutor = @idTutor
    AND c.id_estado = 1 -- Solo comentarios activos
ORDER BY c.fecha DESC";

            var lista = new List<ComentarioTutorInfoDto>();

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@idTutor", idTutor);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var dto = new ComentarioTutorInfoDto
                {
                    Usuario = reader.GetString(reader.GetOrdinal("Usuario")),
                    Comentario = reader.GetString(reader.GetOrdinal("Comentario")),
                    Calificacion = Convert.ToInt32(reader["Calificacion"]),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha"))
                };
                lista.Add(dto);
            }

            return lista;
        }
    }
    }

