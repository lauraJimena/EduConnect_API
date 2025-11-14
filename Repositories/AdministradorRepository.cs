using EduConnect_API.Dtos;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Utilities;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

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
                (nom_usu, apel_usu, id_tipo_ident, num_ident, correo_usu, tel_usu, contras_usu, id_carrera, id_semestre, id_rol, id_estado, cambiar_contras) 
                VALUES (@nom_usu, @apel_usu, @id_tipo_ident, @num_ident, @correo_usu, 
                @tel_usu, @contras_usu, @id_carrera, @id_semestre, @id_rol, @id_estado, @cambiar_contras)";

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
                command.Parameters.AddWithValue("@cambiar_contras", true);


                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar el usuario en la base de datos: " + ex.Message);
            }
        }
        public async Task<IEnumerable<ObtenerUsuarioDto>> ObtenerUsuarios(int? idRol = null, int? idEstado = null, string? numIdent = null)
        {
            var usuarios = new List<ObtenerUsuarioDto>();

            // Consulta base
            var sql = new StringBuilder(@"
        SELECT 
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
            e.nom_estado AS Estado,
            u.id_rol AS IdRol,
            u.id_estado AS IdEstado, 
            u.id_tipo_ident AS IdTipoIdent,
            u.id_carrera AS IdCarrera
        FROM [EduConnect].[dbo].[usuario] AS u
        INNER JOIN [EduConnect].[dbo].[tipo_ident] AS ti ON u.id_tipo_ident = ti.id_tipo_ident
        INNER JOIN [EduConnect].[dbo].[carrera] AS c ON u.id_carrera = c.id_carrera
        INNER JOIN [EduConnect].[dbo].[semestre] AS s ON u.id_semestre = s.id_semestre
        INNER JOIN [EduConnect].[dbo].[rol] AS r ON u.id_rol = r.id_rol
        INNER JOIN [EduConnect].[dbo].[estado] AS e ON u.id_estado = e.id_estado
        WHERE 1=1
    ");

            // Agregar filtros dinámicos
            if (idRol.HasValue)
                sql.Append(" AND u.id_rol = @idRol");

            if (idEstado.HasValue)
                sql.Append(" AND u.id_estado = @idEstado");

            if (!string.IsNullOrWhiteSpace(numIdent))
                sql.Append(" AND u.num_ident LIKE '%' + @numIdent + '%'"); // permite coincidencias parciales

            sql.Append(" ORDER BY u.nom_usu, u.apel_usu;");

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql.ToString(), connection);

                // Parámetros opcionales
                if (idRol.HasValue)
                    command.Parameters.AddWithValue("@idRol", idRol.Value);

                if (idEstado.HasValue)
                    command.Parameters.AddWithValue("@idEstado", idEstado.Value);

                if (!string.IsNullOrWhiteSpace(numIdent))
                    command.Parameters.AddWithValue("@numIdent", numIdent);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    usuarios.Add(new ObtenerUsuarioDto
                    {
                        IdUsu = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                        Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                        Apellido = reader.GetString(reader.GetOrdinal("Apellido")),
                        TipoIdent = reader.GetString(reader.GetOrdinal("TipoIdentificacion")),
                        NumIdent = reader.GetString(reader.GetOrdinal("NumeroIdentificacion")),
                        Correo = reader.GetString(reader.GetOrdinal("Correo")),
                        TelUsu = reader.GetString(reader.GetOrdinal("Telefono")),
                        Carrera = reader.GetString(reader.GetOrdinal("Carrera")),
                        IdSemestre = reader.GetByte(reader.GetOrdinal("Semestre")),
                        Rol = reader.GetString(reader.GetOrdinal("Rol")),
                        Estado = reader.GetString(reader.GetOrdinal("Estado")),
                        IdRol = reader.GetByte(reader.GetOrdinal("IdRol")),
                        IdEstado = reader.GetByte(reader.GetOrdinal("IdEstado")),
                        IdTipoIdent = reader.GetByte(reader.GetOrdinal("IdTipoIdent")),
                        IdCarrera = reader.GetInt16(reader.GetOrdinal("IdCarrera"))


                    });
                }

                return usuarios;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar los usuarios: " + ex.Message);
            }
        }

        public async Task<ObtenerUsuarioDto?> ObtenerUsuarioPorId(int idUsuario)
        {
            const string sql = @"
SELECT
    u.id_usu         AS IdUsu,
    u.nom_usu        AS Nombre,
    u.apel_usu       AS Apellido,
    u.id_tipo_ident  AS IdTipoIdent,
    u.num_ident      AS NumIdent,
    u.correo_usu     AS Correo,
    u.tel_usu        AS TelUsu,
    u.contras_usu    AS ContrasUsu,
    u.id_carrera     AS IdCarrera,
    u.id_semestre    AS IdSemestre,
    u.id_rol         AS IdRol,
    u.id_estado      AS IdEstado,
    u.avatar         AS Avatar 
FROM [EduConnect].[dbo].[usuario] AS u
WHERE u.id_usu = @idUsu;";


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
                        IdUsu = reader.GetInt32(reader.GetOrdinal("IdUsu")),
                        Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                        Apellido = reader.GetString(reader.GetOrdinal("Apellido")),
                        IdTipoIdent = reader.GetByte(reader.GetOrdinal("IdTipoIdent")),
                        NumIdent = reader.GetString(reader.GetOrdinal("NumIdent")),
                        Correo = reader.GetString(reader.GetOrdinal("Correo")),
                        TelUsu = reader.GetString(reader.GetOrdinal("TelUsu")),
                        ContrasUsu = reader.GetString(reader.GetOrdinal("ContrasUsu")),
                        //Manejo seguro de nulos
                        IdCarrera = reader.IsDBNull(reader.GetOrdinal("IdCarrera"))
                        ? (int?)null
                        : reader.GetInt16(reader.GetOrdinal("IdCarrera")),

                        IdSemestre = reader.IsDBNull(reader.GetOrdinal("IdSemestre"))
                        ? (byte?)null
                        : reader.GetByte(reader.GetOrdinal("IdSemestre")),
                        IdRol = reader.GetByte(reader.GetOrdinal("IdRol")),
                        IdEstado = reader.GetByte(reader.GetOrdinal("IdEstado")),
                        Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Avatar"))

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
    id_carrera = @id_carrera,
    id_semestre = @id_semestre,
    id_rol = @id_rol,
    avatar = @avatar
    
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
                command.Parameters.AddWithValue("@id_carrera", usuario.IdCarrera);
                command.Parameters.AddWithValue("@id_semestre", usuario.IdSemestre);
                command.Parameters.AddWithValue("@id_rol", usuario.IdRol);
                command.Parameters.AddWithValue("@avatar", usuario.Avatar);


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
                        SET id_estado = 
                            CASE 
                                WHEN id_estado = 1 THEN 2  -- si está activo inactiva
                                WHEN id_estado = 2 THEN 1  -- si está inactivo activa
                                ELSE id_estado             -- por si acaso, deja igual
                            END
                        WHERE id_usu = @idUsuario;";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@idUsuario", idUsuario);

                return await command.ExecuteNonQueryAsync(); // filas afectadas
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cambiar el estado del usuario: " + ex.Message);
            }
        }

        public async Task<IEnumerable<MateriasDto>> ObtenerTodasMaterias()
        {
            const string sql = @"
SELECT 
    m.id_materia AS IdMateria,
    m.cod_materia AS CodMateria,
    m.nom_materia AS NomMateria,
    m.num_creditos AS NumCreditos,
    m.descrip_materia AS DescripMateria,
    m.id_estado AS IdEstado,
    e.nom_estado AS Estado,
    m.id_semestre AS IdSemestre,
    s.num_semestre AS Semestre,
    m.id_carrera AS IdCarrera,
    c.nom_carrera AS Carrera
FROM [EduConnect].[dbo].[materia] m
INNER JOIN [EduConnect].[dbo].[estado] e ON m.id_estado = e.id_estado
INNER JOIN [EduConnect].[dbo].[semestre] s ON m.id_semestre = s.id_semestre
INNER JOIN [EduConnect].[dbo].[carrera] c ON m.id_carrera = c.id_carrera
ORDER BY m.nom_materia ASC";

            var lista = new List<MateriasDto>();

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var dto = new MateriasDto
                {
                    IdMateria = reader.GetInt32(reader.GetOrdinal("IdMateria")),
                    CodMateria = reader.GetString(reader.GetOrdinal("CodMateria")),
                    NomMateria = reader.GetString(reader.GetOrdinal("NomMateria")),
                    NumCreditos = reader.GetInt32(reader.GetOrdinal("NumCreditos")),
                    DescripMateria = reader.GetString(reader.GetOrdinal("DescripMateria")),
                    IdEstado = Convert.ToInt32(reader["IdEstado"]),
                    Estado = reader.GetString(reader.GetOrdinal("Estado")),
                    IdSemestre = reader.GetInt32(reader.GetOrdinal("IdSemestre")),
                    // CORREGIDO: Convertir num_semestre a string de forma segura
                    Semestre = Convert.ToString(reader["Semestre"]),
                    IdCarrera = reader.GetInt32(reader.GetOrdinal("IdCarrera")),
                    Carrera = reader.GetString(reader.GetOrdinal("Carrera"))
                };
                lista.Add(dto);
            }

            return lista;
        }

        // Obtener materia por ID - CORREGIDO: Conversión segura
        public async Task<MateriasDto> ObtenerMateriaPorId(int idMateria)
        {
            const string sql = @"
SELECT 
    m.id_materia AS IdMateria,
    m.cod_materia AS CodMateria,
    m.nom_materia AS NomMateria,
    m.num_creditos AS NumCreditos,
    m.descrip_materia AS DescripMateria,
    m.id_estado AS IdEstado,
    e.nom_estado AS Estado,
    m.id_semestre AS IdSemestre,
    s.num_semestre AS Semestre,
    m.id_carrera AS IdCarrera,
    c.nom_carrera AS Carrera
FROM [EduConnect].[dbo].[materia] m
INNER JOIN [EduConnect].[dbo].[estado] e ON m.id_estado = e.id_estado
INNER JOIN [EduConnect].[dbo].[semestre] s ON m.id_semestre = s.id_semestre
INNER JOIN [EduConnect].[dbo].[carrera] c ON m.id_carrera = c.id_carrera
WHERE m.id_materia = @IdMateria";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@IdMateria", idMateria);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new MateriasDto
                {
                    IdMateria = reader.GetInt32(reader.GetOrdinal("IdMateria")),
                    CodMateria = reader.GetString(reader.GetOrdinal("CodMateria")),
                    NomMateria = reader.GetString(reader.GetOrdinal("NomMateria")),
                    NumCreditos = reader.GetInt32(reader.GetOrdinal("NumCreditos")),
                    DescripMateria = reader.GetString(reader.GetOrdinal("DescripMateria")),
                    IdEstado = Convert.ToInt32(reader["IdEstado"]),
                    Estado = reader.GetString(reader.GetOrdinal("Estado")),
                    IdSemestre = reader.GetInt32(reader.GetOrdinal("IdSemestre")),

                    Semestre = Convert.ToString(reader["Semestre"]),
                    IdCarrera = reader.GetInt32(reader.GetOrdinal("IdCarrera")),
                    Carrera = reader.GetString(reader.GetOrdinal("Carrera"))
                };
            }

            throw new Exception("Materia no encontrada");
        }


        public async Task<int> CrearMateria(CrearMateriaDto materia)
        {
            const string sql = @"
INSERT INTO [EduConnect].[dbo].[materia] 
    (cod_materia, nom_materia, num_creditos, descrip_materia, id_estado, id_semestre, id_carrera)
VALUES 
    (@cod_materia, @nom_materia, @num_creditos, @descrip_materia, 1, @id_semestre, @id_carrera)";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);

            // QUITAMOS el parámetro @id_materia
            command.Parameters.AddWithValue("@cod_materia", materia.CodMateria);
            command.Parameters.AddWithValue("@nom_materia", materia.NomMateria);
            command.Parameters.AddWithValue("@num_creditos", materia.NumCreditos);
            command.Parameters.AddWithValue("@descrip_materia", materia.DescripMateria);
            command.Parameters.AddWithValue("@id_semestre", materia.IdSemestre);
            command.Parameters.AddWithValue("@id_carrera", materia.IdCarrera);

            return await command.ExecuteNonQueryAsync();
        }


        public async Task<bool> ExisteMateria(int idMateria)
        {
            const string sql = "SELECT COUNT(1) FROM [EduConnect].[dbo].[materia] WHERE id_materia = @id_materia";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_materia", idMateria);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0; // CORREGIDO: Conversión segura
        }

        public async Task<bool> ExisteCodigoMateria(string codMateria, int? idMateriaExcluir = null)
        {
            var sql = "SELECT COUNT(1) FROM [EduConnect].[dbo].[materia] WHERE cod_materia = @cod_materia";

            if (idMateriaExcluir.HasValue)
            {
                sql += " AND id_materia != @id_materia";
            }

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@cod_materia", codMateria);

            if (idMateriaExcluir.HasValue)
            {
                command.Parameters.AddWithValue("@id_materia", idMateriaExcluir.Value);
            }

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0; // CORREGIDO: Conversión segura
        }

        public async Task<bool> ExisteCarrera(int idCarrera)
        {
            const string sql = "SELECT COUNT(1) FROM [EduConnect].[dbo].[carrera] WHERE id_carrera = @id_carrera";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_carrera", idCarrera);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0; // CORREGIDO: Conversión segura
        }

        public async Task<bool> ExisteSemestre(int idSemestre)
        {
            const string sql = "SELECT COUNT(1) FROM [EduConnect].[dbo].[semestre] WHERE id_semestre = @id_semestre";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_semestre", idSemestre);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0; // CORREGIDO: Conversión segura
        }
        // Actualizar materia
        public async Task<int> ActualizarMateria(ActualizarMateriaDto materia)
        {
            const string sql = @"
UPDATE [EduConnect].[dbo].[materia]
SET cod_materia = @cod_materia,
    nom_materia = @nom_materia,
    num_creditos = @num_creditos,
    descrip_materia = @descrip_materia,
    id_estado = @id_estado,
    id_semestre = @id_semestre,
    id_carrera = @id_carrera
WHERE id_materia = @id_materia";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id_materia", materia.IdMateria);
            command.Parameters.AddWithValue("@cod_materia", materia.CodMateria);
            command.Parameters.AddWithValue("@nom_materia", materia.NomMateria);
            command.Parameters.AddWithValue("@num_creditos", materia.NumCreditos);
            command.Parameters.AddWithValue("@descrip_materia", materia.DescripMateria);
            command.Parameters.AddWithValue("@id_estado", materia.IdEstado);
            command.Parameters.AddWithValue("@id_semestre", materia.IdSemestre);
            command.Parameters.AddWithValue("@id_carrera", materia.IdCarrera);

            return await command.ExecuteNonQueryAsync();
        }

        // Cambiar estado de materia (activar/inactivar)
        public async Task<int> CambiarEstadoMateria(int idMateria, int idEstado)
        {
            const string sql = @"
UPDATE [EduConnect].[dbo].[materia]
SET id_estado = @id_estado
WHERE id_materia = @id_materia";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_materia", idMateria);
            command.Parameters.AddWithValue("@id_estado", idEstado);

            return await command.ExecuteNonQueryAsync();
        }

    
    public async Task<IEnumerable<ReporteTutorDto>> ObtenerReporteTutoresAsync()
        {
            var lista = new List<ReporteTutorDto>();

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand("dbo.usp_Reporte_TutoresActivosInactivos", connection);
            command.CommandType = CommandType.StoredProcedure;

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var dto = new ReporteTutorDto
                {
                    IdTutor = reader.GetInt32(reader.GetOrdinal("IdTutor")),
                    NombreTutor = reader.IsDBNull(reader.GetOrdinal("NombreTutor")) ? "Sin nombre" : reader.GetString(reader.GetOrdinal("NombreTutor")),
                    Carrera = reader.IsDBNull(reader.GetOrdinal("Carrera")) ? "Sin carrera" : reader.GetString(reader.GetOrdinal("Carrera")),
                    Semestre = reader.IsDBNull(reader.GetOrdinal("Semestre")) ? 0 : Convert.ToInt32(reader["Semestre"]),
                    Estado = reader.IsDBNull(reader.GetOrdinal("Estado")) ? "Desconocido" : reader.GetString(reader.GetOrdinal("Estado")),
                    CantidadMaterias = reader.IsDBNull(reader.GetOrdinal("CantidadMaterias")) ? 0 : reader.GetInt32(reader.GetOrdinal("CantidadMaterias")),
                    PromedioCalificacion = reader.IsDBNull(reader.GetOrdinal("PromedioCalificacion")) ? 0 : Convert.ToDouble(reader["PromedioCalificacion"])
                };
                lista.Add(dto);
            }

            return lista;
        }
        public async Task<IEnumerable<ReporteTutoradoDto>> ObtenerReporteTutoradosActivosAsync()
        {
            var lista = new List<ReporteTutoradoDto>();

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand("dbo.usp_Reporte_TutoradosActivos", connection);
            command.CommandType = CommandType.StoredProcedure;

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var dto = new ReporteTutoradoDto
                {
                    IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usu")),
                    NombreTutorado = reader.GetString(reader.GetOrdinal("NombreTutorado")),
                    Carrera = reader.GetString(reader.GetOrdinal("Carrera")),
                    TotalTutorias = reader.GetInt32(reader.GetOrdinal("TotalTutorias")),
                    UltimaTutoria = reader["UltimaTutoria"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["UltimaTutoria"]),
                    MateriasMasSolicitadas = reader["MateriasMasSolicitadas"].ToString() ?? "Sin registros"
                };

                lista.Add(dto);
            }

            return lista;
        }

    }
}