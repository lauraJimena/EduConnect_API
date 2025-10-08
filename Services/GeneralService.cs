using EduConnect_API.Dtos;
using EduConnect_API.Repositories;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;
using System.Text.RegularExpressions;

namespace EduConnect_API.Services
{
    public class GeneralService : IGeneralService
    {
        private readonly IGeneralRepository _generalRepository;

        public GeneralService(IGeneralRepository generalRepository)
        {
            _generalRepository = generalRepository;
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

            var result = await _generalRepository.RegistrarUsuario(usuario);

            if (result <= 0)
                throw new Exception("No se pudo registrar el usuario en la base de datos.");
        }
        public async Task<ObtenerUsuarioDto> IniciarSesion(IniciarSesionDto usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.NumIdent))
                throw new Exception("El número de identificación es obligatorio.");
            else if (!Regex.IsMatch(usuario.NumIdent, @"^\d{7,10}$"))
                throw new Exception("El número de identificación que ingresaste no es válido.");

            // Validación de contraseña
            if (string.IsNullOrWhiteSpace(usuario.ContrasUsu))
                throw new Exception("La contraseña es obligatoria.");
           
            // Consultar en BD
            var result = await _generalRepository.IniciarSesion(usuario);

            if (result == null)
                throw new Exception("Número de identificación o contraseña incorrectos.");

            if (result.IdEstado != 1) // Ejemplo: 1 = activo
                throw new Exception("El usuario no está activo, contacte al administrador.");

            return result;

        }
        public async Task<List<CarreraDto>> ObtenerCarrerasAsync()
        {
            // No se necesita validación de parámetros porque no recibe ninguno,
            
            var carreras = await _generalRepository.ObtenerCarrerasAsync();

            if (carreras == null || !carreras.Any())
                throw new InvalidOperationException("No se encontraron carreras registradas.");

            return carreras;
        }
        public async Task<List<TipoIdentidadDto>> ObtenerTiposIdentidadAsync()
        {
            // No se necesita validación de parámetros porque no recibe ninguno,

            var tiposIdent = await _generalRepository.ObtenerTiposIdentidadAsync();

            if (tiposIdent == null || !tiposIdent.Any())
                throw new InvalidOperationException("No se encontraron carreras registradas.");

            return tiposIdent;
        }

    }

}
