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

            const string sql = @"SELECT 
                                u.id_usu AS IdUsuario,
                                u.nom_usu AS Nombre,
                                u.apel_usu AS Apellido,
                                ti.nombre AS TipoIdentificacion,
                                u.num_ident AS NumeroIdentificacion,
                                u.correo_usu AS Correo,
                                u.tel_usu AS Telefono,
                                c.nom_carrera AS Carrera,
                                s.num_semestre AS Semestre,
                                r.nom_rol AS Rol,
                                e.nom_estado AS Estado
                            FROM [EduConnect].[dbo].[usuario] AS u
                            INNER JOIN [EduConnect].[dbo].[tipo_ident] AS ti ON u.id_tipo_ident = ti.id_tipo_ident
                            INNER JOIN [EduConnect].[dbo].[carrera] AS c ON u.id_carrera = c.id_carrera
                            INNER JOIN [EduConnect].[dbo].[semestre] AS s ON u.id_semestre = s.id_semestre
                            INNER JOIN [EduConnect].[dbo].[rol] AS r ON u.id_rol = r.id_rol
                            INNER JOIN [EduConnect].[dbo].[estado] AS e ON u.id_estado = e.id_estado
                            ORDER BY u.nom_usu, u.apel_usu;
                            ";

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
                    TipoIdent= reader.GetString(3),
                    NumIdent = reader.GetString(4),
                    Correo = reader.GetString(5),
                    TelUsu = reader.GetString(6),
                    Carrera = reader.GetString(7),
                    IdSemestre = reader.GetByte(8),
                    Rol = reader.GetString(9),
                    Estado = reader.GetString(10)


                });
            }

            return usuarios;
        }
        public async Task<ObtenerUsuarioDto?> ObtenerUsuarioPorId(int idUsuario)
        {
            const string sql = @"
                                 SELECT
                                 u.id_usu AS IdUsu,
                                    u.nom_usu AS Nombre,
                                    u.apel_usu AS Apellido,
                                    ti.nombre AS TipoIdentificacion,
                                    u.num_ident AS NumeroIdentificacion,
                                    u.correo_usu AS Correo,
                                    u.tel_usu AS Telefono,
                                    u.contras_usu AS Contrasena,
                                    c.nom_carrera AS Carrera,
                                    s.num_semestre AS Semestre,
                                    r.nom_rol AS Rol,
                                    e.nom_estado AS Estado
                                FROM [EduConnect].[dbo].[usuario] AS u
                                INNER JOIN [EduConnect].[dbo].[tipo_ident] AS ti ON u.id_tipo_ident = ti.id_tipo_ident
                                INNER JOIN [EduConnect].[dbo].[carrera] AS c ON u.id_carrera = c.id_carrera
                                INNER JOIN [EduConnect].[dbo].[semestre] AS s ON u.id_semestre = s.id_semestre
                                INNER JOIN [EduConnect].[dbo].[rol] AS r ON u.id_rol = r.id_rol
                                INNER JOIN [EduConnect].[dbo].[estado] AS e ON u.id_estado = e.id_estado
                                WHERE u.id_usu = @idUsu";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@idUsu", idUsuario);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new ObtenerUsuarioDto
                    {
                        IdUsu = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Apellido = reader.GetString(2),
                        TipoIdent = reader.GetString(3),
                        NumIdent = reader.GetString(4),
                        Correo = reader.GetString(5),
                        TelUsu = reader.GetString(6),
                        ContrasUsu = reader.GetString(7),
                        Carrera = reader.GetString(8),
                        IdSemestre = reader.GetByte(9),
                        Rol = reader.GetString(10),
                        Estado = reader.GetString(11)
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
