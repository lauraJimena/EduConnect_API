using System.Threading.Tasks;
using EduConnect_API.Dtos;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface IUsuarioRepositorio
    {
        Task<UsuarioRespuesta?> IniciarSesion(IniciarSesion dto);
    }
}
