using EduConnect_API.Dtos;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Servicios.Interfaces;

namespace EduConnect_API.Servicios
{
    public class UsuarioServicio : IUsuarioServicio
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public UsuarioServicio(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        public async Task<UsuarioRespuesta?> IniciarSesion(IniciarSesion dto)
        {
            return await _usuarioRepositorio.IniciarSesion(dto);
        }
    }
}

