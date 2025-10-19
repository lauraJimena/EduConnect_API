using EduConnect_API.Dtos;
using EduConnect_API.Repositories;
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
        public async Task<IEnumerable<SolicitudTutorDto>> ObtenerSolicitudesTutor(FiltroSolicitudesTutorDto filtro)
        {
            // Validaciones
            if (filtro.IdTutor <= 0)
                throw new ArgumentException("El ID del tutor es obligatorio.");

            if (!await _tutorRepository.ExisteUsuario(filtro.IdTutor))
                throw new ArgumentException("El tutor no existe.");

            int rolUsuario = await _tutorRepository.ObtenerRolUsuario(filtro.IdTutor);
            if (rolUsuario != 2)
                throw new ArgumentException("El usuario no tiene permisos de tutor.");

            return await _tutorRepository.ObtenerSolicitudesTutor(filtro);
        }

        public async Task<int> AceptarSolicitudTutoria(int idTutoria)
        {
            if (idTutoria <= 0)
                throw new ArgumentException("El ID de la tutoría es obligatorio.");

            return await _tutorRepository.AceptarSolicitudTutoria(idTutoria);
        }

        public async Task<int> RechazarSolicitudTutoria(int idTutoria)
        {
            if (idTutoria <= 0)
                throw new ArgumentException("El ID de la tutoría es obligatorio.");

            return await _tutorRepository.RechazarSolicitudTutoria(idTutoria);
        }

        public async Task<DetalleSolicitudTutoriaDto> ObtenerDetalleSolicitud(int idTutoria)
        {
            if (idTutoria <= 0)
                throw new ArgumentException("El ID de la tutoría es obligatorio.");

            return await _tutorRepository.ObtenerDetalleSolicitud(idTutoria);
        }

        public async Task<IEnumerable<MateriaDto>> ObtenerMateriasTutor(int idTutor)
        {
            if (idTutor <= 0)
                throw new ArgumentException("El ID del tutor es obligatorio.");

            if (!await _tutorRepository.ExisteUsuario(idTutor))
                throw new ArgumentException("El tutor no existe.");

            int rolUsuario = await _tutorRepository.ObtenerRolUsuario(idTutor);
            if (rolUsuario != 2)
                throw new ArgumentException("El usuario no tiene permisos de tutor.");

            return await _tutorRepository.ObtenerMateriasTutor(idTutor);
        }


        public Task<IEnumerable<ObtenerMateriaDto>> ListarMateriasAsignadasAsync(int idUsuario)
            => _tutorRepository.ListarMateriasAsignadasAsync(idUsuario);

        public async Task<SeleccionarGuardarMateriaResultadoDto> SeleccionarYGuardarAsync(SeleccionarGuardarMateriaDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));
            if (dto.IdUsuario <= 0) throw new ArgumentException("El ID del tutor es obligatorio.");
            if (dto.IdMaterias == null || dto.IdMaterias.Count == 0)
                throw new ArgumentException("Debe proporcionar al menos una materia.");

            // --- Validar tutor ---
            if (!await _tutorRepository.ExisteUsuario(dto.IdUsuario))
                throw new ArgumentException("El tutor no existe.");

            var rolUsuario = await _tutorRepository.ObtenerRolUsuario(dto.IdUsuario);
            if (rolUsuario != 2)
                throw new ArgumentException("El usuario no tiene permisos de tutor.");

            // 🔹 NUEVO: obtener una sola vez la carrera del tutor
            var idCarreraTutor = await _tutorRepository.ObtenerCarreraDeUsuario(dto.IdUsuario);
            if (idCarreraTutor is null)
                throw new ArgumentException("El tutor no tiene una carrera asociada.");

            // --- Límite actual ---
            var totalActual = await _tutorRepository.ContarMateriasAsignadas(dto.IdUsuario);
            var restantes = Math.Max(0, 5 - totalActual);
            if (restantes == 0)
            {
                return new SeleccionarGuardarMateriaResultadoDto
                {
                    Insertada = false,
                    Mensaje = "Límite alcanzado: ya tienes 5 materias.",
                    Totales = totalActual,
                    Restantes = 0,
                    MateriasAsignadas = await _tutorRepository.ListarMateriasAsignadasAsync(dto.IdUsuario)
                };
            }

            // --- Insertar materias ---
            foreach (var idMateria in dto.IdMaterias)
            {
                if (restantes == 0) break;

                if (!await _tutorRepository.ExisteMateria(idMateria))
                    continue;

                var idCarreraMateria = await _tutorRepository.ObtenerCarreraPorMateria(idMateria);

                // 🔹 NUEVO: validación negocio → materia debe pertenecer a la MISMA carrera del tutor
                if (idCarreraMateria != idCarreraTutor)
                {
                    return new SeleccionarGuardarMateriaResultadoDto
                    {
                        Insertada = false,
                        Mensaje = "La materia no corresponde a la carrera del tutor.",
                        Totales = totalActual,
                        Restantes = restantes,
                        MateriasAsignadas = await _tutorRepository.ListarMateriasAsignadasAsync(dto.IdUsuario)
                    };
                }

                // Duplicado
                if (await _tutorRepository.ExisteAsignacion(dto.IdUsuario, idMateria))
                    continue;

                // Siempre insertar con la carrera de la MATERIA (no la que envíe el cliente)
                await _tutorRepository.SeleccionarYGuardarAsync(new SeleccionarGuardarMateriaDto
                {
                    IdUsuario = dto.IdUsuario,
                    IdMaterias = new List<int> { idMateria },
                    IdCarrera = idCarreraMateria
                });

                totalActual = await _tutorRepository.ContarMateriasAsignadas(dto.IdUsuario);
                restantes = Math.Max(0, 5 - totalActual);
            }

            return new SeleccionarGuardarMateriaResultadoDto
            {
                Insertada = true,
                Mensaje = "Materias asignadas correctamente.",
                Totales = totalActual,
                Restantes = restantes,
                MateriasAsignadas = await _tutorRepository.ListarMateriasAsignadasAsync(dto.IdUsuario)
            };
        }

        public async Task<IEnumerable<ComentarioTutorDto>> ObtenerComentariosTutor(FiltroComentariosTutorDto filtro)
        {
            // Validaciones
            if (filtro.IdTutor <= 0)
                throw new ArgumentException("El ID del tutor es obligatorio.");

            if (!await _tutorRepository.ExisteUsuario(filtro.IdTutor))
                throw new ArgumentException("El tutor no existe.");

            int rolUsuario = await _tutorRepository.ObtenerRolUsuario(filtro.IdTutor);
            if (rolUsuario != 2)
                throw new ArgumentException("El usuario no tiene permisos de tutor.");

            // Validar calificación si se proporciona
            if (filtro.Calificacion.HasValue && (filtro.Calificacion < 1 || filtro.Calificacion > 5))
                throw new ArgumentException("La calificación debe estar entre 1 y 5.");

            return await _tutorRepository.ObtenerComentariosTutor(filtro);
        }
        public async Task<ObtenerUsuarioDto> ObtenerTutorPorIdAsync(int idTutorado)
        {
            try
            {
                return await _tutorRepository.ObtenerTutorPorId(idTutorado);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el perfil del tutor: " + ex.Message);
            }
        }
        public async Task<bool> ValidarMateriasTutorAsync(int idTutor)
        {
            try
            {
                return await _tutorRepository.TutorTieneMateriasAsync(idTutor);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar las materias del tutor: " + ex.Message);
            }
        }
        public async Task<IEnumerable<MateriaDto>> ObtenerMateriasPorTutorAsync(int idTutor)
        {
            if (idTutor <= 0)
                throw new ArgumentException("El ID del tutor es inválido.");

            return await _tutorRepository.ObtenerMateriasPorTutorAsync(idTutor);
        }
        public async Task RegistrarMateriasTutorAsync(RegistrarMateriasTutorDto dto)
        {
            if (dto.IdTutor <= 0)
                throw new ArgumentException("El ID del tutor no es válido.");

            if (dto.Materias == null || !dto.Materias.Any())
                throw new ArgumentException("Debes seleccionar al menos una materia.");

            await _tutorRepository.RegistrarMateriasTutorAsync(dto);
        }





    }
}
