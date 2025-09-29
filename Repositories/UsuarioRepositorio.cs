using EduConnect_API.Dtos;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Utilities;
using Microsoft.Data.SqlClient;

namespace EduConnect_API.Repositories
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly DbContextUtility _dbContextUtility;

        public UsuarioRepositorio(DbContextUtility dbContextUtility)
        {
            _dbContextUtility = dbContextUtility ?? throw new ArgumentNullException(nameof(dbContextUtility));
        }

        // Implementación que coincide con la interfaz
        public async Task<UsuarioRespuesta?> IniciarSesion(IniciarSesion dto)
        {
            const string sql = @"
                SELECT id_usu, nom_usu, apel_usu, num_ident, correo_usu, id_rol, id_estado
                FROM [EduConnect].[dbo].[usuario]
                WHERE num_ident = @num_ident AND contras_usu = @contras_usu AND id_estado = 1";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@num_ident", dto.NumIdent);
            command.Parameters.AddWithValue("@contras_usu", dto.ContrasUsu);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UsuarioRespuesta
                {
                    IdUsu = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Apellido = reader.GetString(2),
                    NumIdent = reader.GetString(3),
                    Correo = reader.GetString(4),
                    IdRol = reader.GetByte(5),
                    IdEstado = reader.GetByte(6)
                };
            }

            return null;
        }
    }
}
