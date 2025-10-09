using EduConnect_API.Dtos;
using EduConnect_API.Dtos;
using EduConnect_API.Repositories;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;
using EduConnect_API.Services.Interfaces;
using System.Text.RegularExpressions;

namespace EduConnect_API.Services
{
    public class TutoradoService : ITutoradoService
    {
        private readonly ITutoradoRepository _tutoradoRepository;

        public TutoradoService(ITutoradoRepository tutoradoRepository)
        {
            _tutoradoRepository = tutoradoRepository;
        }

        public Task<IEnumerable<HistorialTutoriaDto>> ObtenerHistorialAsync(int idTutorado, List<int>? idsEstado)
            => _tutoradoRepository.ObtenerHistorialTutoradoAsync(idTutorado, idsEstado);
        public async Task<int> ActualizarPerfilTutorado(EditarPerfilDto tutorado)
        {
            // Validación de ID
            if (tutorado.IdUsu <= 0)
                throw new ArgumentException("El IdUsuario es obligatorio para actualizar.");

            // Verificar que el usuario existe
            if (!await _tutoradoRepository.ExisteUsuario(tutorado.IdUsu))
                throw new ArgumentException("El tutorado no existe.");

            // Validar que el usuario tiene rol de Tutorado (rol 1)
            int rolUsuario = await _tutoradoRepository.ObtenerRolUsuario(tutorado.IdUsu);
            if (rolUsuario != 1)
                throw new ArgumentException("El usuario no tiene permisos de tutorado para editar este perfil.");

            // Resto de validaciones...
            if (string.IsNullOrWhiteSpace(tutorado.Nombre))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(tutorado.Apellido))
                throw new ArgumentException("El apellido es obligatorio.");

            if (tutorado.IdTipoIdent <= 0)
                throw new ArgumentException("El tipo de identificación es obligatorio.");

            if (string.IsNullOrWhiteSpace(tutorado.NumIdent))
                throw new ArgumentException("El número de identificación es obligatorio.");
            else if (!Regex.IsMatch(tutorado.NumIdent, @"^\d{7,10}$"))
                throw new ArgumentException("El número de identificación debe tener entre 7 y 10 dígitos.");

            if (string.IsNullOrWhiteSpace(tutorado.TelUsu))
                throw new ArgumentException("El teléfono es obligatorio.");
            else if (!Regex.IsMatch(tutorado.TelUsu, @"^3\d{9}$"))
                throw new ArgumentException("El teléfono debe tener 10 dígitos y empezar en 3.");

            if (string.IsNullOrWhiteSpace(tutorado.Correo))
                throw new ArgumentException("El correo es obligatorio.");
            else if (!Regex.IsMatch(tutorado.Correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("El correo no tiene un formato válido.");

            return await _tutoradoRepository.ActualizarPerfil(tutorado);
        }
        public async Task<IEnumerable<SolicitudTutoriaDto>> ObtenerSolicitudesTutorias(FiltroSolicitudesDto filtro)
        {
            // Validaciones
            if (filtro.IdTutorado <= 0)
                throw new ArgumentException("El ID del tutorado es obligatorio.");

            if (!await _tutoradoRepository.ExisteUsuario(filtro.IdTutorado))
                throw new ArgumentException("El tutorado no existe.");

            // Validar que el usuario tiene rol de Tutorado (rol 1)
            int rolUsuario = await _tutoradoRepository.ObtenerRolUsuario(filtro.IdTutorado);
            if (rolUsuario != 1)
                throw new ArgumentException("El usuario no tiene permisos de tutorado.");

            return await _tutoradoRepository.ObtenerSolicitudesTutorias(filtro);
        }

        public async Task<IEnumerable<EstadoSolicitudDto>> ObtenerEstadosSolicitud()
        {
            return await _tutoradoRepository.ObtenerEstadosSolicitud();
        }
        public async Task<int> CrearSolicitudTutoria(SolicitudTutoriaRequestDto solicitud)
        {
            // Validaciones básicas
            if (solicitud.IdTutorado <= 0)
                throw new ArgumentException("El ID del tutorado es obligatorio.");

            if (solicitud.IdTutor <= 0)
                throw new ArgumentException("El ID del tutor es obligatorio.");

            if (solicitud.IdMateria <= 0)
                throw new ArgumentException("El ID de la materia es obligatorio.");

            if (solicitud.IdModalidad <= 0)
                throw new ArgumentException("El ID de la modalidad es obligatorio.");

            // Validar que el tutorado existe y es tutorado
            if (!await _tutoradoRepository.ExisteUsuario(solicitud.IdTutorado))
                throw new ArgumentException("El tutorado no existe.");

            int rolUsuario = await _tutoradoRepository.ObtenerRolUsuario(solicitud.IdTutorado);
            if (rolUsuario != 1)
                throw new ArgumentException("El usuario no tiene permisos de tutorado.");

            // Validar que el tutor existe y es tutor
            if (!await _tutoradoRepository.ExisteTutor(solicitud.IdTutor))
                throw new ArgumentException("El tutor no existe o no tiene permisos de tutor.");

            // Validar que la materia existe
            if (!await _tutoradoRepository.ExisteMateria(solicitud.IdMateria))
                throw new ArgumentException("La materia no existe.");

            // Validar que la modalidad existe
            if (!await _tutoradoRepository.ExisteModalidad(solicitud.IdModalidad))
                throw new ArgumentException("La modalidad no existe.");

            // Validar fecha (no puede ser en el pasado)
            if (solicitud.Fecha < DateTime.Today)
                throw new ArgumentException("La fecha no puede ser en el pasado.");

            // Validar formato de hora
            if (string.IsNullOrWhiteSpace(solicitud.Hora))
                throw new ArgumentException("La hora es obligatoria.");

            // Validar que la hora tenga formato correcto (HH:mm)
            if (!TimeSpan.TryParse(solicitud.Hora, out TimeSpan hora))
                throw new ArgumentException("El formato de la hora no es válido. Use formato HH:mm (ej: 14:30).");

            // Validar tema (obligatorio)
            if (string.IsNullOrWhiteSpace(solicitud.Tema))
                throw new ArgumentException("El tema es obligatorio.");

            if (solicitud.Tema.Length < 5)
                throw new ArgumentException("El tema debe tener al menos 5 caracteres.");

            // Validar comentario adicional (opcional pero si existe, validar longitud)
            if (!string.IsNullOrWhiteSpace(solicitud.ComentarioAdicional) && solicitud.ComentarioAdicional.Length > 500)
                throw new ArgumentException("El comentario adicional no puede exceder los 500 caracteres.");

            return await _tutoradoRepository.CrearSolicitudTutoria(solicitud);
        }
    }
}