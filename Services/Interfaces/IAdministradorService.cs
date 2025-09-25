using EduConnect_API.Dtos;

namespace EduConnect_API.Services.Interfaces
{
    public interface IAdministradorService
    {
        
        Task RegistrarUsuario(CrearUsuarioDto usuario);
        Task<IEnumerable<ObtenerUsuarioDto>> ObtenerUsuarios();
        // Consultar por ID
        Task<ObtenerUsuarioDto> ObtenerUsuarioPorId(int idUsuario);
        Task<int> ActualizarUsuario(ActualizarUsuarioDto usuario);
        Task<int> EliminarUsuario(int IdUsuario);
    }
}
