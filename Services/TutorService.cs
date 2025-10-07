using EduConnect_API.Dtos;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;
using System.Text.RegularExpressions;

namespace EduConnect_API.Services
{
    public class TutorService : ITutorService
    {
        private readonly ITutorRepository _tutorRepository;

        public TutorService(ITutorRepository repo) => _tutorRepository = repo;

        // ✅ Validar que el id del tutor sea obligatorio
        public async Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialAsync(int idTutor, List<int>? estados)
        {
            if (idTutor <= 0)
                throw new ArgumentException("El ID del tutor es obligatorio y debe ser mayor que 0.");

            return await _tutorRepository.ObtenerHistorialTutorAsync(idTutor, estados);
        }

        public Task<IEnumerable<ObtenerTutorDto>> ObtenerTutoresAsync(BuscarTutorDto filtros)
            => _tutorRepository.ObtenerTutoresAsync(filtros);

        public async Task<int> ActualizarPerfilTutor(EditarPerfilDto tutor)
        {
            // Validación de ID
            if (tutor.IdUsu <= 0)
                throw new ArgumentException("El IdUsuario es obligatorio para actualizar.");

            // Verificar que el usuario existe
            if (!await _tutorRepository.ExisteUsuario(tutor.IdUsu))
                throw new ArgumentException("El tutor no existe.");

            // Validar que el usuario tiene rol de Tutor (rol 2)
            int rolUsuario = await _tutorRepository.ObtenerRolUsuario(tutor.IdUsu);
            if (rolUsuario != 2)
                throw new ArgumentException("El usuario no tiene permisos de tutor para editar este perfil.");

            // Resto de validaciones...
            if (string.IsNullOrWhiteSpace(tutor.Nombre))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(tutor.Apellido))
                throw new ArgumentException("El apellido es obligatorio.");

            if (tutor.IdTipoIdent <= 0)
                throw new ArgumentException("El tipo de identificación es obligatorio.");

            if (string.IsNullOrWhiteSpace(tutor.NumIdent))
                throw new ArgumentException("El número de identificación es obligatorio.");
            else if (!Regex.IsMatch(tutor.NumIdent, @"^\d{7,10}$"))
                throw new ArgumentException("El número de identificación debe tener entre 7 y 10 dígitos.");

            if (string.IsNullOrWhiteSpace(tutor.TelUsu))
                throw new ArgumentException("El teléfono es obligatorio.");
            else if (!Regex.IsMatch(tutor.TelUsu, @"^3\d{9}$"))
                throw new ArgumentException("El teléfono debe tener 10 dígitos y empezar en 3.");

            if (string.IsNullOrWhiteSpace(tutor.Correo))
                throw new ArgumentException("El correo es obligatorio.");
            else if (!Regex.IsMatch(tutor.Correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("El correo no tiene un formato válido.");

            return await _tutorRepository.ActualizarPerfilTutor(tutor);
        }
    }
}