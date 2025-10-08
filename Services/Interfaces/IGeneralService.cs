using EduConnect_API.Dtos;

namespace EduConnect_API.Services.Interfaces
{
    public interface IGeneralService 
    {
        Task RegistrarUsuario(CrearUsuarioDto usuario);
        Task<ObtenerUsuarioDto> IniciarSesion(IniciarSesionDto usuario);
        Task<List<CarreraDto>> ObtenerCarrerasAsync();
        Task<List<TipoIdentidadDto>> ObtenerTiposIdentidadAsync();
    }
}
