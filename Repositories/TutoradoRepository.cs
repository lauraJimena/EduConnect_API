using EduConnect_API.Dtos;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Utilities;
using Microsoft.Data.SqlClient;
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
                WHERE t.id_tutorado = @idTutorado
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
    }
}
