using EduConnect_API.Dtos;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface IGeneralRepository
    {
        Task<int> RegistrarUsuario(CrearUsuarioDto usuario);
        Task<ObtenerUsuarioDto> IniciarSesion(IniciarSesionDto usuario);
        Task<List<CarreraDto>> ObtenerCarrerasAsync();
        Task<List<TipoIdentidadDto>> ObtenerTiposIdentidadAsync();

    }
}
