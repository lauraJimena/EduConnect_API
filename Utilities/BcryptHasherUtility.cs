using Org.BouncyCastle.Crypto.Generators;

namespace EduConnect_API.Utilities
{
    public class BcryptHasherUtility
    {
        private readonly int _workFactor = 12; // nivel de costo (entre 10 y 14 es seguro)

        // Hashear la contraseña
        public string Hash(string contrasena)
        {
            if (string.IsNullOrWhiteSpace(contrasena))
                throw new ArgumentException("La contraseña no puede estar vacía.", nameof(contrasena));

            return BCrypt.Net.BCrypt.HashPassword(contrasena, _workFactor);
        }

        // Verificar una contraseña
        public bool Verify(string contrasena, string Contrasenahashed)
        {
            if (string.IsNullOrEmpty(Contrasenahashed))
                return false;

            return BCrypt.Net.BCrypt.Verify(contrasena, Contrasenahashed);
        }
    }
}
