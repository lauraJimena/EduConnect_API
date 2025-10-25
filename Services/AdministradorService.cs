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
        //public async Task<IEnumerable<ObtenerUsuarioDto>> ObtenerUsuarios()
        //{
        //    var usuarios = await _administradorRepository.ObtenerUsuarios();


        //    if (usuarios == null || !usuarios.Any())
        //    {
        //        // En vez de devolver null, lanza excepción controlada
        //        throw new Exception("No se encontraron usuarios en la base de datos.");
        //    }

        //    return usuarios;
        //}
        public async Task<IEnumerable<ObtenerUsuarioDto>> ObtenerUsuariosAsync(int? idRol = null, int? idEstado = null, string? numIdent = null)
        {
            return await _administradorRepository.ObtenerUsuarios(idRol, idEstado, numIdent);
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
        public async Task<IEnumerable<MateriasDto>> ObtenerTodasMaterias()
        {
            return await _administradorRepository.ObtenerTodasMaterias();
        }

        // CONSULTAR: Obtener materia por ID
        public async Task<MateriasDto> ObtenerMateriaPorId(int idMateria)
        {
            if (idMateria <= 0)
                throw new ArgumentException("El ID de la materia es obligatorio.");

            if (!await _administradorRepository.ExisteMateria(idMateria))
                throw new ArgumentException("La materia no existe.");

            return await _administradorRepository.ObtenerMateriaPorId(idMateria);
        }

        // CREAR: Crear nueva materia
        public async Task<int> CrearMateria(CrearMateriaDto materia)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(materia.CodMateria))
                throw new ArgumentException("El código de la materia es obligatorio.");

            if (string.IsNullOrWhiteSpace(materia.NomMateria))
                throw new ArgumentException("El nombre de la materia es obligatorio.");

            if (materia.NumCreditos <= 0)
                throw new ArgumentException("El número de créditos debe ser mayor a 0.");

            if (materia.IdSemestre <= 0)
                throw new ArgumentException("El semestre es obligatorio.");

            if (materia.IdCarrera <= 0)
                throw new ArgumentException("La carrera es obligatoria.");

            // Validar formato del código (ej: MAT101)
            if (!Regex.IsMatch(materia.CodMateria, @"^[A-Z]{3}\d{3}$"))
                throw new ArgumentException("El código de materia debe tener 3 letras seguidas de 3 números (ej: MAT101).");

            // Validar que no exista otro código igual
            if (await _administradorRepository.ExisteCodigoMateria(materia.CodMateria))
                throw new ArgumentException("Ya existe una materia con este código.");

            // Validar que exista la carrera
            if (!await _administradorRepository.ExisteCarrera(materia.IdCarrera))
                throw new ArgumentException("La carrera especificada no existe.");

            // Validar que exista el semestre
            if (!await _administradorRepository.ExisteSemestre(materia.IdSemestre))
                throw new ArgumentException("El semestre especificado no existe.");

            return await _administradorRepository.CrearMateria(materia);
        }

        // ACTUALIZAR: Actualizar materia existente
        public async Task<int> ActualizarMateria(ActualizarMateriaDto materia)
        {
            // Validaciones
            if (materia.IdMateria <= 0)
                throw new ArgumentException("El ID de la materia es obligatorio.");

            if (!await _administradorRepository.ExisteMateria(materia.IdMateria))
                throw new ArgumentException("La materia no existe.");

            if (string.IsNullOrWhiteSpace(materia.CodMateria))
                throw new ArgumentException("El código de la materia es obligatorio.");

            if (string.IsNullOrWhiteSpace(materia.NomMateria))
                throw new ArgumentException("El nombre de la materia es obligatorio.");

            if (materia.NumCreditos <= 0)
                throw new ArgumentException("El número de créditos debe ser mayor a 0.");

            if (materia.IdEstado <= 0)
                throw new ArgumentException("El estado es obligatorio.");

            if (materia.IdSemestre <= 0)
                throw new ArgumentException("El semestre es obligatorio.");

            if (materia.IdCarrera <= 0)
                throw new ArgumentException("La carrera es obligatoria.");

            // Validar formato del código
            if (!Regex.IsMatch(materia.CodMateria, @"^[A-Z]{3}\d{3}$"))
                throw new ArgumentException("El código de materia debe tener 3 letras seguidas de 3 números (ej: MAT101).");

            // Validar que no exista otro código igual (excluyendo la materia actual)
            if (await _administradorRepository.ExisteCodigoMateria(materia.CodMateria, materia.IdMateria))
                throw new ArgumentException("Ya existe otra materia con este código.");

            // Validar que exista la carrera
            if (!await _administradorRepository.ExisteCarrera(materia.IdCarrera))
                throw new ArgumentException("La carrera especificada no existe.");

            // Validar que exista el semestre
            if (!await _administradorRepository.ExisteSemestre(materia.IdSemestre))
                throw new ArgumentException("El semestre especificado no existe.");

            return await _administradorRepository.ActualizarMateria(materia);
        }

        // ELIMINAR: Inactivar materia
        public async Task<int> InactivarMateria(int idMateria)
        {
            if (idMateria <= 0)
                throw new ArgumentException("El ID de la materia es obligatorio.");

            if (!await _administradorRepository.ExisteMateria(idMateria))
                throw new ArgumentException("La materia no existe.");

            return await _administradorRepository.CambiarEstadoMateria(idMateria, 2); // Estado 2 = Inactivo
        }

        // ACTIVAR: Activar materia
        public async Task<int> ActivarMateria(int idMateria)
        {
            if (idMateria <= 0)
                throw new ArgumentException("El ID de la materia es obligatorio.");

            if (!await _administradorRepository.ExisteMateria(idMateria))
                throw new ArgumentException("La materia no existe.");

            return await _administradorRepository.CambiarEstadoMateria(idMateria, 1); // Estado 1 = Activo
        }

        public async Task<IEnumerable<ReporteTutorDto>> ObtenerReporteTutoresAsync()
        {
            return await _administradorRepository.ObtenerReporteTutoresAsync();
        }
        public async Task<IEnumerable<ReporteTutoradoDto>> ObtenerReporteTutoradosActivosAsync()
        {
            return await _administradorRepository.ObtenerReporteTutoradosActivosAsync();
        }



    }
}
