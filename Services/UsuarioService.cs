using EduConnect_API.Dtos;
using EduConnect_API.Repositories;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }
        public async Task RegistrarUsuario(UsuarioDto usuario)
        {

            
            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                throw new Exception("El nombre del usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(usuario.Apellido))
                throw new Exception("El apellido del usuario es obligatorio.");

            if (usuario.IdTipoIdent <= 0)
                throw new Exception("Debe seleccionar un tipo de usuario válido.");

            
            var result = await _usuarioRepository.RegistrarUsuario(usuario);

            // Si el repositorio devuelve 0, significa que no se insertó
            if (result <= 0)
                throw new Exception("No se pudo registrar el usuario en la base de datos."); ;
        }
        public async Task<IEnumerable<UsuarioDto>> ObtenerUsuarios()
        {
            var usuarios = await _usuarioRepository.ObtenerUsuarios();

            
            if (usuarios == null || !usuarios.Any())
            {
                // En vez de devolver null, lanza excepción controlada
                throw new Exception("No se encontraron usuarios en la base de datos.");
            }

            return usuarios;
        }
    }
}
