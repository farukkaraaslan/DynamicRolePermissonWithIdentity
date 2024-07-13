using Castle.DynamicProxy;
using Core.Extension;
using Core.Utilities.Exceptions;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Business.BusinessAspects;

public class SecuredOperation : MethodInterception
{
    private string[] _roles;
    private IHttpContextAccessor _httpContextAccessor;

    public SecuredOperation(string roles)
    {
        _roles = roles.Split(',');
        _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();

    }

    protected override void OnBefore(IInvocation invocation)
    {
        var roleClaims = _httpContextAccessor.HttpContext.User.ClaimRoles().Select(r => r.ToLower());
        var permissionClaims = _httpContextAccessor.HttpContext.User.ClaimPermissions().Select(p => p.ToLower());

        foreach (var roleOrPermission in _roles.Select(r => r.ToLower()))
        {
            if (roleClaims.Contains(roleOrPermission) || permissionClaims.Contains(roleOrPermission))
            {
                return;
            }
        }

        throw new AuthorizationException("Access Denied!");
    }
}
