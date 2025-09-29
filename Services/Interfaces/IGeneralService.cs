using EduConnect_API.Dtos;

namespace EduConnect_API.Services.Interfaces
{
    public interface IGeneralService
    {
        Task RegistrarUsuario(CrearUsuarioDto usuario);
        Task<UsuarioRespuesta?> IniciarSesion(IniciarSesion dto);
    }
}
