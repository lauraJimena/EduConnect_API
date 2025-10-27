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
        public async Task<ObtenerUsuarioDto> IniciarSesion(IniciarSesionDto usuario)
        {
            const string sql = @"SELECT 
                        u.id_usu AS IdUsuario,
                        u.id_estado, u.id_rol,
                        u.contras_usu AS ContrasenaHash
                    FROM [EduConnect].[dbo].[usuario] AS u WHERE 
                        u.num_ident = @num_ident 
                        AND u.id_estado = 1";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@num_ident", usuario.NumIdent);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ObtenerUsuarioDto
                {
                    IdUsu = reader.GetInt32(0),
                    //Nombre = reader.GetString(1),
                    //Apellido = reader.GetString(2),
                    //NumIdent = reader.GetString(3),
                    //Correo = reader.GetString(4),
                    //Rol = reader.GetString(5),
                    //Estado = reader.GetString(6),
                    //Carrera = reader.GetString(7),
                    //IdRol= reader.GetByte(8), 
                    IdEstado = reader.GetByte(1),
                    IdRol= reader.GetByte(2),
                    ContrasenaHash = reader.GetString(3)


                };
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

    }
}
