using EduConnect_API.Dtos;
using EduConnect_API.Utilities;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace EduConnect_API.Repositories
{
    public class ChatsRepository : IChatsRepository
    {
        private readonly DbContextUtility _dbContextUtility;
        public ChatsRepository(DbContextUtility dbContextUtility)
        {
            _dbContextUtility = dbContextUtility ?? throw new ArgumentNullException(nameof(dbContextUtility));
        }
        public async Task<int> CrearChat(CrearChatDto chat)
        {
            const string sql = @"
                INSERT INTO [EduConnect].[dbo].[chat] 
                (id_tutoria, fecha_creacion) 
                VALUES (@id_tutoria, @fecha_creacion)";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@id_tutoria", chat.IdTutoria);
                command.Parameters.AddWithValue("@fecha_creacion", chat.FechaCreacion);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar el Chat en la base de datos: " + ex.Message);
            }
        }

        public async Task<int> CrearMensaje(CrearMensajeDto mensaje)
        {
            const string sql = @"
                INSERT INTO [EduConnect].[dbo].[mensaje] 
                (id_chat, id_usu, contenido, fecha_envio) 
                VALUES (@id_chat, @id_usu, @contenido, @fecha_envio)";

            try
            {
                using var connection = _dbContextUtility.GetOpenConnection();
                using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@id_chat", mensaje.IdChat);
                command.Parameters.AddWithValue("@id_usu", mensaje.IdEmisor);
                command.Parameters.AddWithValue("@contenido", mensaje.Contenido);
                command.Parameters.AddWithValue("@fecha_envio", mensaje.FechaEnvio);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar el Mensaje en la base de datos: " + ex.Message);
            }
        }

        public async Task<IEnumerable<ObtenerChatDto>> ObtenerChatsPorUsuario(int usuarioId)
        {
            var chats = new List<ObtenerChatDto>();

            var rolUsuario = await ObtenerRolUsuarioAsync(usuarioId);

            string sql;

            if (rolUsuario == "Tutorado")
            {
                sql = @"
        SELECT 
            c.id_chat, 
            c.id_tutoria, 
            c.fecha_creacion,
            u.nom_usu + ' ' + u.apel_usu AS NombreReceptor,  -- El Tutor
            m.nom_materia AS NombreMateria
        FROM chat c
        INNER JOIN tutoria t ON c.id_tutoria = t.id_tutoria
        INNER JOIN usuario u ON u.id_usu = t.id_tutor         -- Tutor
        INNER JOIN materia m ON m.id_materia = t.id_materia
        WHERE t.id_tutorado = @idUsuario";
            }
            else if (rolUsuario == "Tutor")
            {
                sql = @"
        SELECT 
            c.id_chat, 
            c.id_tutoria, 
            c.fecha_creacion,
            u.nom_usu + ' ' + u.apel_usu AS NombreReceptor,  -- El Tutorado
            m.nom_materia AS NombreMateria
        FROM chat c
        INNER JOIN tutoria t ON c.id_tutoria = t.id_tutoria
        INNER JOIN usuario u ON u.id_usu = t.id_tutorado      -- Tutorado
        INNER JOIN materia m ON m.id_materia = t.id_materia
        WHERE t.id_tutor = @idUsuario";
            }
            else
            {
                return chats; // No hay chats para otros roles
            }

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@idUsuario", usuarioId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                chats.Add(new ObtenerChatDto
                {
                    IdChat = reader.GetInt32(0),
                    IdTutoria = reader.GetInt32(1),
                    FechaCreacion = reader.GetDateTime(2),
                    NombreReceptor = reader.GetString(3),
                    NombreMateria = reader.GetString(4)
                });
            }

            return chats;
        }


        public async Task<string> ObtenerRolUsuarioAsync(int usuarioId)
        {
            const string sql = @"
        SELECT r.nom_rol
        FROM usuario u
        INNER JOIN rol r ON u.id_rol = r.id_rol
        WHERE u.id_usu = @usuarioId";

            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@usuarioId", usuarioId);

            var rol = (string?)await command.ExecuteScalarAsync();
            return rol ?? "Ninguno";
        }

        public async Task<IEnumerable<ObtenerMensajeDto>> ObtenerMensajes(int idChat)
        {
            var mensajes = new List<ObtenerMensajeDto>();
            const string sql = @"
                SELECT id_mensaje, id_chat, id_usu, contenido, fecha_envio
                FROM mensaje
                WHERE id_chat = @idChat
                ORDER BY fecha_envio DESC";
            using var connection = _dbContextUtility.GetOpenConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@idChat", idChat);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                mensajes.Add(new ObtenerMensajeDto
                {
                    IdMensaje = reader.GetInt32(0),
                    IdChat = reader.GetInt32(1),
                    IdEmisor = reader.GetInt32(2),
                    Contenido = reader.GetString(3),
                    FechaEnvio = reader.GetDateTime(4)
                });
            }
            return mensajes;
        }

    }
}
