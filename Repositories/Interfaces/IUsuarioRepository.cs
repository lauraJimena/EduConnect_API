using EduConnect_API.Dtos;

namespace EduConnect_API.Repositories.Interfaces
{

        public interface IUsuarioRepository
        {
            Task<int> RegistrarUsuario(UsuarioDto usuario);
            Task<IEnumerable<UsuarioDto>> ObtenerUsuarios();
    }
    
}
