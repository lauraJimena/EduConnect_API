using EduConnect_API.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EduConnect_API.Utilities
{
    public class JwtUtility
    {
        /// <summary></summary>
        public static RespuestaInicioSesionDto? GenTokenkey(RespuestaInicioSesionDto userToken, JwtSettingsDto jwtSettings)
        {
            try
            {
                if (userToken == null) throw new ArgumentException(nameof(userToken));

                // Obtén la clave secreta
                var key = System.Text.Encoding.ASCII.GetBytes(jwtSettings.IssuerSigningKey);

                DateTime expireTime = DateTime.Now;
                if (jwtSettings.FlagExpirationTimeHours)
                {
                    expireTime = DateTime.Now.AddHours(jwtSettings.ExpirationTimeHours);
                }
                else if (jwtSettings.FlagExpirationTimeMinutes)
                {
                    expireTime = DateTime.Now.AddMinutes(jwtSettings.ExpirationTimeMinutes);
                }
                else
                {
                    return null;
                }

                // Definir las reclamaciones
                var claims = new List<Claim>
                {
                    new Claim("TiempoExpiracion", expireTime.ToString("yyyy-MM-dd HH:mm:ss")),                    
                    new Claim("IdUsuario", userToken.IdUsuario.ToString()),
                    new Claim("IdRol", userToken.IdRol.ToString())

                };

                // Generar el token JWT
                var JWTToken = new JwtSecurityToken(
                    issuer: jwtSettings.ValidIssuer,
                    audience: jwtSettings.ValidAudience,
                    claims: claims,
                    notBefore: DateTime.Now,
                    expires: expireTime,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                );

                // Asignar el token generado
                userToken.Token = new JwtSecurityTokenHandler().WriteToken(JWTToken);
                userToken.TiempoExpiracion = expireTime;

                return userToken;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
