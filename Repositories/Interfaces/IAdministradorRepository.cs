using EduConnect_API.Dtos;

namespace EduConnect_API.Repositories.Interfaces
{

        public interface IAdministradorRepository
        {
            Task<int> RegistrarUsuario(CrearUsuarioDto usuario);
            Task<IEnumerable<ObtenerUsuarioDto>> ObtenerUsuarios();
            Task<int> ActualizarUsuario(ActualizarUsuarioDto usuario);
            Task<int> EliminarUsuario(int IdUsuario);
           Task<ObtenerUsuarioDto?> ObtenerUsuarioPorId(int idUsuario);

    }
    
}
