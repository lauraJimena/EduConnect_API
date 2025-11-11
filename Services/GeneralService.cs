using Azure.Core;
using EduConnect_API.Dtos;
using EduConnect_API.Repositories;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;
using EduConnect_API.Utilities;
using Org.BouncyCastle.Crypto.Generators;
using System.Text.RegularExpressions;

namespace EduConnect_API.Services
{
    public class GeneralService : IGeneralService
    {
        private readonly IGeneralRepository _generalRepository;
        private readonly JwtSettingsDto _jwtSettings;
        private readonly BcryptHasherUtility _hasher;

        public GeneralService(IGeneralRepository generalRepository, JwtSettingsDto jwtSettings, BcryptHasherUtility hasher)
        {
            _generalRepository = generalRepository;
            _jwtSettings = jwtSettings;
            _hasher = hasher;
        }
        public async Task RegistrarUsuario(CrearUsuarioDto usuario)
        {
            // Validaciones básicas obligatorias
            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                throw new Exception("El nombre del usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(usuario.Apellido))
                throw new Exception("El apellido del usuario es obligatorio.");

            if (usuario.IdTipoIdent <= 0)
                throw new Exception("Debe seleccionar un tipo de identificación válido.");

            if (string.IsNullOrWhiteSpace(usuario.NumIdent))
                throw new Exception("El número de identificación es obligatorio.");

            // Validación de correo
            if (string.IsNullOrWhiteSpace(usuario.Correo))
                throw new Exception("El correo es obligatorio.");
            else if (!Regex.IsMatch(usuario.Correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new Exception("El formato del correo no es válido.");

            // Validación de número de identificación
            if (string.IsNullOrWhiteSpace(usuario.NumIdent))
                throw new Exception("El número de identificación es obligatorio.");
            else if (!Regex.IsMatch(usuario.NumIdent, @"^\d{7,10}$"))
                throw new Exception("El número de identificación que ingresaste no es válido.");

            // Validación de teléfono (10 dígitos numéricos)
            if (string.IsNullOrWhiteSpace(usuario.TelUsu))
                throw new Exception("El teléfono es obligatorio.");
            else if (!Regex.IsMatch(usuario.TelUsu, @"^[0-9]{10}$"))
                throw new Exception("El teléfono debe tener 10 dígitos.");

            // Validación de contraseña
            if (string.IsNullOrWhiteSpace(usuario.ContrasUsu))
                throw new Exception("La contraseña es obligatoria.");
            else if (!Regex.IsMatch(usuario.ContrasUsu, @"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{7,}$"))
                throw new Exception("La contraseña debe tener al menos 7 caracteres, incluir una mayúscula, un número y un carácter especial.");

            // Hashear la contraseña antes de guardarla
            usuario.ContrasUsu = _hasher.Hash(usuario.ContrasUsu);

            // Validaciones de llaves foráneas
            if (usuario.IdCarrera <= 0)
                throw new Exception("Debe seleccionar una carrera válida.");

            if (usuario.IdSemestre <= 0)
                throw new Exception("Debe seleccionar un semestre válido.");

            if (usuario.IdRol <= 0)
                throw new Exception("Debe asignar un rol válido.");

          
            if (await _generalRepository.ExisteNumeroIdentificacion(usuario.NumIdent))
                throw new Exception("Ya existe un usuario registrado con este número de identificación.");

          
            if (await _generalRepository.ExisteCorreo(usuario.Correo))
                throw new Exception("Ya existe un usuario registrado con este correo electrónico.");

            var result = await _generalRepository.RegistrarUsuario(usuario);

            if (result <= 0)
                throw new Exception("No se pudo registrar el usuario en la base de datos.");
        }
        public async Task<RespuestaInicioSesionDto> IniciarSesion(IniciarSesionDto usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.NumIdent))
                throw new Exception("El número de identificación es obligatorio.");
            else if (!Regex.IsMatch(usuario.NumIdent, @"^\d{7,10}$"))
                throw new Exception("El número de identificación que ingresaste no es válido.");

            // Validación de contraseña
            if (string.IsNullOrWhiteSpace(usuario.ContrasUsu))
                throw new Exception("La contraseña es obligatoria.");

            //request.Contras = EncryptUtility.EncryptPassword(request.Contra);

            RespuestaInicioSesionDto respuesta = new();
            var result= await _generalRepository.IniciarSesion(usuario);

            if (result == null)
                throw new Exception("Número de identificación o contraseña incorrectos.");

            if (result.IdEstado != 1) //  1 = activo
                throw new Exception("El usuario no está activo");

            var ok = _hasher.Verify(usuario.ContrasUsu, result.ContrasenaHash);
            if (!ok)
                throw new Exception("Número de identificación o contraseña incorrectos.");

            respuesta.IdUsuario = result.IdUsu; // Agregar el ID antes de crear el token
            respuesta.IdRol = result.IdRol;
            respuesta = JwtUtility.GenTokenkey(respuesta, _jwtSettings);
            if (respuesta == null)
                throw new Exception("Error al generar el token de autenticación. Verifique la configuración JWT.");

            // Continuar solo si todo está OK
            respuesta.Respuesta = 1;
            respuesta.Mensaje = "Inicio de sesión exitoso";

            return respuesta;

            //// Consultar en BD
            //var result = await _generalRepository.IniciarSesion(usuario);

            //if (result == null)
            //    throw new Exception("Número de identificación o contraseña incorrectos.");

            //if (result.IdEstado != 1) // Ejemplo: 1 = activo
            //    throw new Exception("El usuario no está activo, contacte al administrador.");

            //return result;

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
