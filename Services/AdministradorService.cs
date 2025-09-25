using EduConnect_API.Dtos;
using EduConnect_API.Repositories;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;
using System.Text.RegularExpressions;

namespace EduConnect_API.Services
{
    public class AdministradorService : IAdministradorService
    {
        private readonly IAdministradorRepository _administradorRepository;

        public AdministradorService(IAdministradorRepository administradorRepository)
        {
            _administradorRepository = administradorRepository;
        }
        public async Task RegistrarUsuario(CrearUsuarioDto usuario)
        {
            //Validaciones básicas obligatorias
            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                throw new Exception("El nombre del usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(usuario.Apellido))
                throw new Exception("El apellido del usuario es obligatorio.");

            if (usuario.IdTipoIdent <= 0)
                throw new Exception("Debe seleccionar un tipo de identificación válido.");

            if (string.IsNullOrWhiteSpace(usuario.NumIdent))
                throw new Exception("El número de identificación es obligatorio.");


            //Validación de correo
            if (string.IsNullOrWhiteSpace(usuario.Correo))
                throw new Exception("El correo es obligatorio.");
            else if (!Regex.IsMatch(usuario.Correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new Exception("El formato del correo no es válido.");
            // Validación de número de identificación
            if (string.IsNullOrWhiteSpace(usuario.NumIdent))
                throw new Exception("El número de identificación es obligatorio.");
            else if (!Regex.IsMatch(usuario.NumIdent, @"^\d{7,10}$"))
                throw new Exception("El número de identificación que ingresaste no es válido.");

            //Validación de teléfono (10 dígitos numéricos)
            if (string.IsNullOrWhiteSpace(usuario.TelUsu))
                throw new Exception("El teléfono es obligatorio.");
            else if (!Regex.IsMatch(usuario.TelUsu, @"^[0-9]{10}$"))
                throw new Exception("El teléfono debe tener 10 dígitos.");

            // Validación de contraseña
            if (string.IsNullOrWhiteSpace(usuario.ContrasUsu))
                throw new Exception("La contraseña es obligatoria.");
            else if (!Regex.IsMatch(usuario.ContrasUsu, @"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{7,}$"))
                throw new Exception("La contraseña debe tener al menos 7 caracteres, incluir una mayúscula, un número y un carácter especial.");

            //Validaciones de llaves foráneas
            if (usuario.IdCarrera <= 0)
                throw new Exception("Debe seleccionar una carrera válida.");

            if (usuario.IdSemestre <= 0)
                throw new Exception("Debe seleccionar un semestre válido.");

            if (usuario.IdRol <= 0)
                throw new Exception("Debe asignar un rol válido.");

            var result = await _administradorRepository.RegistrarUsuario(usuario);

            if (result <= 0)
                throw new Exception("No se pudo registrar el usuario en la base de datos.");
        }
        public async Task<IEnumerable<ObtenerUsuarioDto>> ObtenerUsuarios()
        {
            var usuarios = await _administradorRepository.ObtenerUsuarios();

            
            if (usuarios == null || !usuarios.Any())
            {
                // En vez de devolver null, lanza excepción controlada
                throw new Exception("No se encontraron usuarios en la base de datos.");
            }

            return usuarios;
        }
        public async Task<ObtenerUsuarioDto> ObtenerUsuarioPorId(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El IdUsuario es inválido.");

            var usuario = await _administradorRepository.ObtenerUsuarioPorId(idUsuario);

            if (usuario == null)
                throw new KeyNotFoundException("No se encontró el usuario con el Id especificado.");

            return usuario;
        }

        public async Task<int> ActualizarUsuario(ActualizarUsuarioDto usuario)
        {
            if (usuario.IdUsu <= 0)
                throw new ArgumentException("El IdUsuario es obligatorio para actualizar.");

            // Validación de nombre
            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                throw new ArgumentException("El nombre es obligatorio.");

            // Validación de apellido
            if (string.IsNullOrWhiteSpace(usuario.Apellido))
                throw new ArgumentException("El apellido es obligatorio.");

            // Validación de número de identificación (7 a 10 dígitos)
            if (string.IsNullOrWhiteSpace(usuario.NumIdent))
                throw new ArgumentException("El número de identificación es obligatorio.");
            else if (!Regex.IsMatch(usuario.NumIdent, @"^\d{7,10}$"))
                throw new ArgumentException("El número de identificación debe tener entre 7 y 10 dígitos.");

            // Validación de teléfono colombiano (10 dígitos y empieza en 3)
            if (string.IsNullOrWhiteSpace(usuario.TelUsu))
                throw new ArgumentException("El teléfono es obligatorio.");
            else if (!Regex.IsMatch(usuario.TelUsu, @"^3\d{9}$"))
                throw new ArgumentException("El teléfono debe tener 10 dígitos y empezar en 3.");

            // Validación de correo electrónico
            if (string.IsNullOrWhiteSpace(usuario.Correo))
                throw new ArgumentException("El correo es obligatorio.");
            else if (!Regex.IsMatch(usuario.Correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("El correo no tiene un formato válido.");

            // Validación de contraseña (si se envía una nueva)
            if (!string.IsNullOrWhiteSpace(usuario.ContrasUsu))
            {
                if (usuario.ContrasUsu.Length < 7 ||
                    !Regex.IsMatch(usuario.ContrasUsu, @"[A-Z]") ||
                    !Regex.IsMatch(usuario.ContrasUsu, @"[0-9]") ||
                    !Regex.IsMatch(usuario.ContrasUsu, @"[!@#$%^&*(),.?""{}|<>]"))
                {
                    throw new ArgumentException("La contraseña debe tener al menos 7 caracteres, incluir una mayúscula, un número y un carácter especial.");
                }
            }

            return await _administradorRepository.ActualizarUsuario(usuario);
        }
        public async Task<int> EliminarUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El IdUsuario es inválido.");

            var filasAfectadas = await _administradorRepository.EliminarUsuario(idUsuario);

            if (filasAfectadas == 0)
                throw new InvalidOperationException("El usuario ya está inactivo o no existe.");


            return filasAfectadas;
        }


    }
}
