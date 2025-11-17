using EduConnect_API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Services.Interfaces
{
    public interface IGeneralService 
    {
        Task<int> RegistrarUsuario(CrearUsuarioDto usuario);
        Task<RespuestaInicioSesionDto> IniciarSesion(IniciarSesionDto usuario);
        Task<List<CarreraDto>> ObtenerCarrerasAsync();
        Task<List<TipoIdentidadDto>> ObtenerTiposIdentidadAsync();
        Task<int> ActualizarPasswordAsync(int idUsuario, string nuevaPassword);
        Task<bool> EnviarCorreoRecuperacionAsync(string correo);
        Task<(bool Ok, string Msg)> RestablecerContrasenaAsync(RestablecerContrasenaDto dto);
        Task<bool> EnviarCorreoBienvenidaAsync(int idUsu);
    }
}
