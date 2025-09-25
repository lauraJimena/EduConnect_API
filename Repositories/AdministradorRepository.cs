using EduConnect_API.Dtos;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Utilities;
using Microsoft.Data.SqlClient;

namespace EduConnect_API.Repositories
{
    public class AdministradorRepository : IAdministradorRepository
    {
        private readonly DbContextUtility _dbContextUtility;

        public AdministradorRepository(DbContextUtility dbContextUtility)
        {
            _dbContextUtility = dbContextUtility ?? throw new ArgumentNullException(nameof(dbContextUtility));
        }
        public async Task<int> RegistrarUsuario(CrearUsuarioDto usuario)
        {
            const string sql = @"
                INSERT INTO [EduConnect].[dbo].[usuario] 
                (nom_usu, apel_usu, id_tipo_ident, num_ident, correo_usu, tel_usu, contras_usu, id_carrera, id_semestre, id_rol, id_estado) 
                VALUES (@nom_usu, @apel_usu, @id_tipo_ident, @num_ident, @correo_usu, 
                @tel_usu, @contras_usu, @id_carrera, @id_semestre, @id_rol, @id_estado)";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@nom_usu", usuario.Nombre);
                command.Parameters.AddWithValue("@apel_usu", usuario.Apellido);
                command.Parameters.AddWithValue("@id_tipo_ident", usuario.IdTipoIdent);
                command.Parameters.AddWithValue("@num_ident", usuario.NumIdent);
                command.Parameters.AddWithValue("@correo_usu", usuario.Correo);
                command.Parameters.AddWithValue("@tel_usu", usuario.TelUsu);
                command.Parameters.AddWithValue("@contras_usu", usuario.ContrasUsu);
                command.Parameters.AddWithValue("@id_carrera", usuario.IdCarrera);
                command.Parameters.AddWithValue("@id_semestre", usuario.IdSemestre);
                command.Parameters.AddWithValue("@id_rol", usuario.IdRol);
                command.Parameters.AddWithValue("@id_estado", 1); // Estado activo por defecto


                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar el usuario en la base de datos: " + ex.Message);
            }
        }
        public async Task<IEnumerable<ObtenerUsuarioDto>> ObtenerUsuarios()
        {
            var usuarios = new List<ObtenerUsuarioDto>();

            const string sql = @"SELECT id_usu, nom_usu, apel_usu, id_tipo_ident, num_ident, correo_usu, tel_usu, id_carrera, 
                                id_semestre, id_rol, id_estado FROM [EduConnect].[dbo].[usuario]";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                usuarios.Add(new ObtenerUsuarioDto
                {
                    IdUsu = reader.GetInt32(0),
                    Nombre = reader.GetString(1),                    
                    Apellido = reader.GetString(2),
                    IdTipoIdent = reader.GetByte(3),
                    NumIdent = reader.GetString(4),
                    Correo = reader.GetString(5),
                    TelUsu = reader.GetString(6),
                    IdCarrera = reader.IsDBNull(7) ? 0 : reader.GetInt16(7),
                    IdSemestre = reader.IsDBNull(8) ? 0 : reader.GetByte(8),
                    IdRol = reader.GetByte(9),
                    IdEstado = reader.GetByte(10)


                });
            }

            return usuarios;
        }
        public async Task<ObtenerUsuarioDto?> ObtenerUsuarioPorId(int idUsuario)
        {
            const string sql = @"
    SELECT id_usu, nom_usu, apel_usu, id_tipo_ident, num_ident, 
           correo_usu, tel_usu, contras_usu, id_carrera, id_semestre, 
           id_rol, id_estado
    FROM [EduConnect].[dbo].[usuario]
    WHERE id_usu = @idUsuario";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@idUsuario", idUsuario);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new ObtenerUsuarioDto
                    {
                        IdUsu = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Apellido = reader.GetString(2),
                        IdTipoIdent = reader.GetByte(3),
                        NumIdent = reader.GetString(4),
                        Correo = reader.GetString(5),
                        TelUsu = reader.GetString(6),
                        ContrasUsu = reader.GetString(7),
                        IdCarrera = reader.GetInt16(8),
                        IdSemestre = reader.GetByte(9),
                        IdRol = reader.GetByte(10),
                        IdEstado = reader.GetByte(11)
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar el usuario: " + ex.Message);
            }
        }

        public async Task<int> ActualizarUsuario(ActualizarUsuarioDto usuario)
        {
            const string sql = @"
        UPDATE [EduConnect].[dbo].[usuario]
        SET nom_usu = @nom_usu,
            apel_usu = @apel_usu,
            id_tipo_ident = @id_tipo_ident,
            num_ident = @num_ident,
            correo_usu = @correo_usu,
            tel_usu = @tel_usu,
            contras_usu = @contras_usu,
            id_carrera = @id_carrera,
            id_semestre = @id_semestre,
            id_rol = @id_rol,
            id_estado = @id_estado
        WHERE id_usu = @id_usu";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@id_usu", usuario.IdUsu);
                command.Parameters.AddWithValue("@nom_usu", usuario.Nombre);
                command.Parameters.AddWithValue("@apel_usu", usuario.Apellido);
                command.Parameters.AddWithValue("@id_tipo_ident", usuario.IdTipoIdent);
                command.Parameters.AddWithValue("@num_ident", usuario.NumIdent);
                command.Parameters.AddWithValue("@correo_usu", usuario.Correo);
                command.Parameters.AddWithValue("@tel_usu", usuario.TelUsu);
                command.Parameters.AddWithValue("@contras_usu", usuario.ContrasUsu);
                command.Parameters.AddWithValue("@id_carrera", usuario.IdCarrera);
                command.Parameters.AddWithValue("@id_semestre", usuario.IdSemestre);
                command.Parameters.AddWithValue("@id_rol", usuario.IdRol);
                command.Parameters.AddWithValue("@id_estado", usuario.IdEstado);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el usuario: " + ex.Message);
            }
        }
        public async Task<int> EliminarUsuario(int idUsuario)
        {
            const string sql = @"
                                UPDATE [EduConnect].[dbo].[usuario]
                                SET id_estado = 2 
                                WHERE id_usu = @idUsuario AND id_estado <> 2";  // Solo cambia si no está inactivo

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@idUsuario", idUsuario);

                return await command.ExecuteNonQueryAsync(); // filas afectadas
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar (inactivar) el usuario: " + ex.Message);
            }
        }



    }
}
