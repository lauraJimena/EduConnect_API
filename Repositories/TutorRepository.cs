using EduConnect_API.Dtos;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Utilities;
using Microsoft.Data.SqlClient;
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
    }
}
