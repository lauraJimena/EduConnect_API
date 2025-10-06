using EduConnect_API.Dtos;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Utilities;
using Microsoft.Data.SqlClient;
using System.Data;
namespace EduConnect_API.Repositories
{
    public class TutorRepository : ITutorRepository
    {
        private readonly DbContextUtility _dbContextUtility;

        public TutorRepository(DbContextUtility dbContextUtility)
        {
            _dbContextUtility = dbContextUtility;
        }

        public async Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialTutorAsync(int idTutor)
        {
            var lista = new List<HistorialTutoriaDto>();

            const string sql = @"
                SELECT id_tutoria, fecha, hora, id_modalidad, tema, comentario_adic, id_tutorado, id_tutor, id_materia, id_estado
                FROM [EduConnect].[dbo].[tutoria]
                WHERE id_tutor = @idTutor
                ORDER BY fecha DESC, hora DESC;";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@idTutor", idTutor);

            using var reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows) return lista;

            var oIdTut = reader.GetOrdinal("id_tutoria");
            var oFecha = reader.GetOrdinal("fecha");
            var oHora = reader.GetOrdinal("hora");
            var oIdModal = reader.GetOrdinal("id_modalidad");
            var oTema = reader.GetOrdinal("tema");
            var oComent = reader.GetOrdinal("comentario_adic");
            var oIdTutorado = reader.GetOrdinal("id_tutorado");
            var oIdTutor = reader.GetOrdinal("id_tutor");
            var oIdMateria = reader.GetOrdinal("id_materia");
            var oIdEstado = reader.GetOrdinal("id_estado");

            while (await reader.ReadAsync())
            {
                lista.Add(new HistorialTutoriaDto
                {
                    IdTutoria = reader.IsDBNull(oIdTut) ? 0 : reader.GetInt32(oIdTut),
                    Fecha = reader.IsDBNull(oFecha) ? DateTime.MinValue : reader.GetDateTime(oFecha),
                    Hora = reader.IsDBNull(oHora) ? TimeSpan.Zero : reader.GetFieldValue<TimeSpan>(oHora),
                    IdModalidad = reader.IsDBNull(oIdModal) ? (byte)0 : reader.GetByte(oIdModal),
                    Tema = reader.IsDBNull(oTema) ? null : reader.GetString(oTema),
                    ComentarioAdic = reader.IsDBNull(oComent) ? null : reader.GetString(oComent),
                    IdTutorado = reader.IsDBNull(oIdTutorado) ? 0 : reader.GetInt32(oIdTutorado),
                    IdTutor = reader.IsDBNull(oIdTutor) ? 0 : reader.GetInt32(oIdTutor),
                    IdMateria = reader.IsDBNull(oIdMateria) ? 0 : reader.GetInt32(oIdMateria),
                    IdEstado = reader.IsDBNull(oIdEstado) ? (byte)0 : reader.GetByte(oIdEstado)
                });
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
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using var command = new SqlCommand(sql, (SqlConnection)connection);

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
