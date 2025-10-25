using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EduConnect_API.Utilities
{
    public class RolesUtility
    {
    }
}
public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly int _rolRequerido;

    public CustomAuthorizeAttribute(int rolRequerido)
    {
        _rolRequerido = rolRequerido;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var rolClaim = user.Claims.FirstOrDefault(c => c.Type == "IdRol")?.Value;

        if (rolClaim == null || rolClaim != _rolRequerido.ToString())
        {
            context.Result = new ForbidResult();
        }
    }
}
