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
    }
}

   