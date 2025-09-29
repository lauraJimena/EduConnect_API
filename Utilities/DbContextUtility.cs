
using Microsoft.Data.SqlClient;
using System;
namespace EduConnect_API.Utilities
{
    public class DbContextUtility
    {
        static readonly string SERVER = "FELIPE_GARAVITO";
        static readonly string DB_NAME = "EduConnect";

        static readonly string Conn = $"Server={SERVER};Database={DB_NAME};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
        public SqlConnection GetOpenConnection()
        {
            var connection = new SqlConnection(Conn);
            try
            {
                Console.WriteLine("Abriendo conexión con la cadena:");
                Console.WriteLine(Conn); // Verifica que se está usando esta cadena

                connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al abrir la conexión: {ex.Message}");
                throw;
            }
            return connection;
        }
    }
}
