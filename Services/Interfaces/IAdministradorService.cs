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

        Task<IEnumerable<MateriasDto>> ObtenerTodasMaterias();
        Task<MateriasDto> ObtenerMateriaPorId(int idMateria);
        Task<int> CrearMateria(CrearMateriaDto materia);
        Task<int> ActualizarMateria(ActualizarMateriaDto materia);
        Task<int> ActivarMateria(int idMateria);
        Task<int> InactivarMateria(int idMateria);

    }
}
