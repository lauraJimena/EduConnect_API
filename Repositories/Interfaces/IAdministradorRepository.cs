using EduConnect_API.Dtos;

namespace EduConnect_API.Repositories.Interfaces
{

        public interface IAdministradorRepository
        {
            Task<int> RegistrarUsuario(CrearUsuarioDto usuario);
            Task<IEnumerable<ObtenerUsuarioDto>> ObtenerUsuarios(int? idRol, int? idEstado, string? numIdent);
        
        Task<int> ActualizarUsuario(ActualizarUsuarioDto usuario);
            Task<int> EliminarUsuario(int IdUsuario);
           Task<ObtenerUsuarioDto?> ObtenerUsuarioPorId(int idUsuario);
        Task<IEnumerable<MateriasDto>> ObtenerTodasMaterias();
        Task<MateriasDto> ObtenerMateriaPorId(int idMateria);
        Task<int> CrearMateria(CrearMateriaDto materia);
        Task<int> ActualizarMateria(ActualizarMateriaDto materia);
        Task<int> CambiarEstadoMateria(int idMateria, int idEstado);
        Task<bool> ExisteMateria(int idMateria);
        Task<bool> ExisteCodigoMateria(string codMateria, int? idMateriaExcluir = null);
        Task<bool> ExisteCarrera(int idCarrera);
        Task<bool> ExisteSemestre(int idSemestre);
    }

}
    

