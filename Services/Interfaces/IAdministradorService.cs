using EduConnect_API.Dtos;

namespace EduConnect_API.Services.Interfaces
{
    public interface IAdministradorService
    {
        
        Task RegistrarUsuario(CrearUsuarioDto usuario);
        Task<IEnumerable<ObtenerUsuarioDto>> ObtenerUsuariosAsync(int? idRol, int? idEstado, string? numIdent);
        // Consultar por ID
        Task<ObtenerUsuarioDto> ObtenerUsuarioPorId(int idUsuario);
        Task<int> ActualizarUsuario(ActualizarUsuarioDto usuario);
        Task<int> EliminarUsuario(int IdUsuario);
        
    }
}
