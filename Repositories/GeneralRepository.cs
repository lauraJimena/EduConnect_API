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
    u.nom_usu AS Nombre,
    u.apel_usu AS Apellido,
    u.num_ident AS NumeroIdent,
    u.correo_usu AS Correo,
    r.nom_rol AS Rol,
    e.nom_estado AS Estado, 
    c.nom_carrera AS Carrera,
	r.id_rol,
    u.id_estado
FROM [EduConnect].[dbo].[usuario] AS u
INNER JOIN [EduConnect].[dbo].[rol] AS r ON u.id_rol = r.id_rol
INNER JOIN [EduConnect].[dbo].[estado] AS e ON u.id_estado = e.id_estado
INNER JOIN [EduConnect].[dbo].[carrera] AS c ON u.id_carrera = c.id_carrera
WHERE 
    u.num_ident = num_ident 
    AND u.contras_usu = @contras_usu
    AND u.id_estado = 1";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@num_ident", usuario.NumIdent);
            command.Parameters.AddWithValue("@contras_usu", usuario.ContrasUsu);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ObtenerUsuarioDto
                {
                    IdUsu = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Apellido = reader.GetString(2),
                    NumIdent = reader.GetString(3),
                    Correo = reader.GetString(4),
                    Rol = reader.GetString(5),
                    Estado = reader.GetString(6),
                    Carrera = reader.GetString(7),
                    IdRol= reader.GetByte(8), 
                    IdEstado = reader.GetByte(9)

                };
            }

            return null;
        }
    }
}
