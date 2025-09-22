using EduConnect_API.Dtos;

namespace EduConnect_API.Services.Interfaces
{
    public interface IUsuarioService
    {
        
        Task RegistrarUsuario(UsuarioDto usuario);
        Task<IEnumerable<UsuarioDto>> ObtenerUsuarios();
    }
}
