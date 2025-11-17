using EduConnect_API.Dtos;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface IGeneralRepository
    {
        Task<int> RegistrarUsuario(CrearUsuarioDto usuario);
        Task<ObtenerUsuarioDto> IniciarSesion(IniciarSesionDto usuario);
        Task<List<CarreraDto>> ObtenerCarrerasAsync();
        Task<List<TipoIdentidadDto>> ObtenerTiposIdentidadAsync();

        Task<bool> ExisteNumeroIdentificacion(string numIdent);
        Task<bool> ExisteCorreo(string correo);
        Task<int> ActualizarDebeActualizarPassword(int idUsuario, bool valor);
        Task<int> ActualizarPassword(int idUsuario, string nuevaPassword);
        Task<string> ObtenerPasswordActualAsync(int idUsuario);
       
       
        Task<UsuarioPorCorreoDto?> ObtenerUsuarioPorCorreoAsync(string correo);
        Task GuardarTokenRecuperacionAsync(string correo, string token);

        Task<TokenRecuperacionDto?> ObtenerTokenRecuperacionAsync(string token);
        Task ActualizarPasswordPorCorreo(string correo, string nuevaPasswordHash);
        Task MarcarTokenUsado(string token);
        Task<DatosBienvenidaDto?> ObtenerDatosBienvenidaAsync(int idUsu);
    }
}
