using EduConnect_API.Dtos;
using EduConnect_API.Utilities;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace EduConnect_API.Repositories
{
    public class GeneralRepository  : IGeneralRepository
    {
        private readonly DbContextUtility _dbContextUtility;
        public GeneralRepository(DbContextUtility dbContextUtility)
        {
            _dbContextUtility = dbContextUtility ?? throw new ArgumentNullException(nameof(dbContextUtility));
        }
        public async Task<DatosBienvenidaDto?> ObtenerDatosBienvenidaAsync(int idUsu)
        {
            const string sql = @"
        SELECT nom_usu AS Nombre, correo_usu
        FROM usuario
        WHERE id_usu = @idUsu";

            using var con = _dbContextUtility.GetOpenConnection();
            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@idUsu", idUsu);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return new DatosBienvenidaDto
            {
                Nombre = reader.GetString(0),
                Correo = reader.GetString(1)
            };
        }

        //public async Task<int> RegistrarUsuario(CrearUsuarioDto usuario)
        //{
        //    const string sql = @"
        //        INSERT INTO [EduConnect].[dbo].[usuario] 
        //        (nom_usu, apel_usu, id_tipo_ident, num_ident, correo_usu, tel_usu, contras_usu, id_carrera, id_semestre, id_rol, id_estado) 
        //        VALUES (@nom_usu, @apel_usu, @id_tipo_ident, @num_ident, @correo_usu, 
        //        @tel_usu, @contras_usu, @id_carrera, @id_semestre, @id_rol, @id_estado)";

        //    try
        //    {
        //        using var connection = _dbContextUtility.GetOpenConnection();
        //        using var command = new SqlCommand(sql, connection);

        //        command.Parameters.AddWithValue("@nom_usu", usuario.Nombre);
        //        command.Parameters.AddWithValue("@apel_usu", usuario.Apellido);
        //        command.Parameters.AddWithValue("@id_tipo_ident", usuario.IdTipoIdent);
        //        command.Parameters.AddWithValue("@num_ident", usuario.NumIdent);
        //        command.Parameters.AddWithValue("@correo_usu", usuario.Correo);
        //        command.Parameters.AddWithValue("@tel_usu", usuario.TelUsu);
        //        command.Parameters.AddWithValue("@contras_usu", usuario.ContrasUsu);
        //        command.Parameters.AddWithValue("@id_carrera", usuario.IdCarrera);
        //        command.Parameters.AddWithValue("@id_semestre", usuario.IdSemestre);
        //        command.Parameters.AddWithValue("@id_rol", usuario.IdRol);
        //        command.Parameters.AddWithValue("@id_estado", 1); // Estado activo por defecto


        //        return await command.ExecuteNonQueryAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error al registrar el usuario en la base de datos: " + ex.Message);
        //    }
        //}
        public async Task<int> RegistrarUsuario(CrearUsuarioDto usuario)
        {
            const string sql = @"
        INSERT INTO [EduConnect].[dbo].[usuario] 
        (nom_usu, apel_usu, id_tipo_ident, num_ident, correo_usu, tel_usu, contras_usu, id_carrera, id_semestre, id_rol, id_estado) 
        VALUES (@nom_usu, @apel_usu, @id_tipo_ident, @num_ident, @correo_usu, 
        @tel_usu, @contras_usu, @id_carrera, @id_semestre, @id_rol, @id_estado);

        SELECT SCOPE_IDENTITY();
    ";

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
                command.Parameters.AddWithValue("@id_estado", 1);

                // 🔥 Obtener el ID insertado
                object result = await command.ExecuteScalarAsync();

                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar el usuario en la base de datos: " + ex.Message);
            }
        }

        public async Task<ObtenerUsuarioDto> IniciarSesion(IniciarSesionDto usuario)
        {
            const string sql = @"
                            SELECT 
                                u.id_usu AS IdUsu,
                                u.id_estado AS IdEstado,
                                u.id_rol AS IdRol,
                                u.contras_usu AS ContrasenaHash, 
                                u.cambiar_contras AS DebeActualizarPassword
                            FROM [EduConnect].[dbo].[usuario] AS u
                            WHERE u.num_ident = @num_ident 
                              AND u.id_estado = 1";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@num_ident", usuario.NumIdent);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var dto = new ObtenerUsuarioDto
                {
                    IdUsu = Convert.ToInt32(reader["IdUsu"]),
                    IdEstado = Convert.ToInt32(reader["IdEstado"]),
                    IdRol = Convert.ToInt32(reader["IdRol"]),
                    ContrasenaHash = reader["ContrasenaHash"].ToString(),
                    DebeActualizarPassword = reader["DebeActualizarPassword"] != DBNull.Value
                    && Convert.ToBoolean(reader["DebeActualizarPassword"])
                };

                return dto;
            }

            return null;
        }

        public async Task<List<CarreraDto>> ObtenerCarrerasAsync()
        {
            const string sql = @"
                            SELECT 
                                c.id_carrera      AS IdCarrera,
                                c.nom_carrera     AS NombreCarrera,
                                c.id_facultad     AS IdFacultad
                            FROM [EduConnect].[dbo].[carrera] AS c
                            WHERE c.nom_carrera <> 'Institucional';";

            try
            {
                
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                using var reader = await command.ExecuteReaderAsync();

                var carreras = new List<CarreraDto>();

                while (await reader.ReadAsync())
                {
                    var carrera = new CarreraDto
                    {
                        IdCarrera = reader.GetInt16(reader.GetOrdinal("IdCarrera")),
                        NombreCarrera = reader.GetString(reader.GetOrdinal("NombreCarrera")),
                        IdFacultad = reader.GetInt16(reader.GetOrdinal("IdFacultad"))
                    };

                    carreras.Add(carrera);
                }

                return carreras;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar las carreras: " + ex.Message);
            }
        }
        public async Task<List<TipoIdentidadDto>> ObtenerTiposIdentidadAsync()
        {
            const string sql = @"
                            SELECT
                                id_tipo_ident AS IdTipoIdent,
                                nombre        AS Nombre
                            FROM [EduConnect].[dbo].[tipo_ident];";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                using var reader = await command.ExecuteReaderAsync();

                var tiposIdentidad = new List<TipoIdentidadDto>();

                while (await reader.ReadAsync())
                {
                    var tipo = new TipoIdentidadDto
                    {
                        IdTipoIdent = reader.GetByte(reader.GetOrdinal("IdTipoIdent")),
                        Nombre = reader.GetString(reader.GetOrdinal("Nombre"))
                    };

                    tiposIdentidad.Add(tipo);
                }

                return tiposIdentidad;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar los tipos de identidad: " + ex.Message);
            }
        }
        public async Task<bool> ExisteNumeroIdentificacion(string numIdent)
        {
            const string sql = "SELECT COUNT(1) FROM [EduConnect].[dbo].[usuario] WHERE num_ident = @numIdent";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@numIdent", numIdent);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> ExisteCorreo(string correo)
        {
            const string sql = "SELECT COUNT(1) FROM [EduConnect].[dbo].[usuario] WHERE correo_usu = @correo";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@correo", correo);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }
        public async Task<int> ActualizarDebeActualizarPassword(int idUsuario, bool valor)
        {
            const string sql = @"
                                UPDATE [EduConnect].[dbo].[usuario]
                                SET cambiar_contras = @valor
                                WHERE id_usu = @id_usu";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@id_usu", idUsuario);
                command.Parameters.AddWithValue("@valor", valor);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el estado de cambio de contraseña: " + ex.Message);
            }
        }
        public async Task<int> ActualizarPassword(int idUsuario, string nuevaPassword)
        {
            const string sql = @"
                                UPDATE [EduConnect].[dbo].[usuario]
                                SET contras_usu= @nuevaPassword,
                                    cambiar_contras = 0
                                WHERE id_usu = @id_usu";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@id_usu", idUsuario);
                command.Parameters.AddWithValue("@nuevaPassword", nuevaPassword);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la contraseña del usuario: " + ex.Message);
            }
        }
        public async Task<string?> ObtenerPasswordActualAsync(int idUsuario)
        {
            const string sql = "SELECT contras_usu FROM [EduConnect].[dbo].[usuario] WHERE id_usu = @id_usu";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_usu", idUsuario);

            var result = await command.ExecuteScalarAsync();
            return result == DBNull.Value ? null : result?.ToString();
        }
        public async Task<UsuarioPorCorreoDto?> ObtenerUsuarioPorCorreoAsync(string correo)
        {
            const string sql = @"
        SELECT 
            id_usu AS IdUsu,
            nom_usu AS Nombre,
            apel_usu AS Apellido,
            correo_usu AS Correo
        FROM [EduConnect].[dbo].[usuario]
        WHERE correo_usu = @correo";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@correo", correo);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UsuarioPorCorreoDto
                {
                    IdUsu = Convert.ToInt32(reader["IdUsu"]),
                    Nombre = reader["Nombre"].ToString()!,
                    Apellido = reader["Apellido"].ToString()!,
                    Correo = reader["Correo"].ToString()!
                };
            }

            return null;
        }
        public async Task GuardarTokenRecuperacionAsync(string correo, string token)
        {
            const string sql = @"
        INSERT INTO [EduConnect].[dbo].[recuperar_password]
            (correo, token, fecha_creacion, fecha_expira, usado)
        VALUES
            (@correo, @token, GETDATE(), DATEADD(MINUTE, 30, GETDATE()), 0)";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@correo", correo);
            command.Parameters.AddWithValue("@token", token);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<TokenRecuperacionDto?> ObtenerTokenRecuperacionAsync(string token)
        {
            const string sql = @"
        SELECT TOP 1 
            correo,
            fecha_expira AS FechaExpira,
            usado AS Usado
        FROM [EduConnect].[dbo].[recuperar_password]
        WHERE token = @token";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@token", token);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new TokenRecuperacionDto
                {
                    Correo = reader["correo"].ToString()!,
                    FechaExpira = Convert.ToDateTime(reader["FechaExpira"]),
                    Usado = Convert.ToBoolean(reader["Usado"])
                };
            }

            return null;
        }

        public async Task ActualizarPasswordPorCorreo(string correo, string nuevaPasswordHash)
        {
            const string sql = @"UPDATE [EduConnect].[dbo].[usuario]
                         SET contras_usu = @contras_usu
                         WHERE correo_usu = @correo";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@correo", correo);
            command.Parameters.AddWithValue("@contras_usu", nuevaPasswordHash);

            await command.ExecuteNonQueryAsync();
        }

        public async Task MarcarTokenUsado(string token)
        {
            const string sql = @"UPDATE [EduConnect].[dbo].[recuperar_password] 
                         SET usado = 1 
                         WHERE token = @token";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@token", token);

            await command.ExecuteNonQueryAsync();
        }



    }
}
