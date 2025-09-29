using EduConnect_API.Dtos;

namespace EduConnect_API.Servicios.Interfaces
{
    public interface IUsuarioServicio
    {
        Task<UsuarioRespuesta?> IniciarSesion(IniciarSesion dto);
    }
}
