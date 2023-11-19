using Business.Abstract;
using Business.Concrete;
using Core.Entities.Concrete;
using Core.Utilities.Security.JWT;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using Autofac.Extras.DynamicProxy;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Business.DependencyResolver;

public class AutofacBusinessModule : Module
{
    private readonly IConfiguration _configuration;

    public AutofacBusinessModule(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    protected override void Load(ContainerBuilder builder)
    {
     

        builder.RegisterType<JwtHelper>().As<ITokenHelper>().SingleInstance();
        
        builder.RegisterType<AppUserManager>().As<IUserService>().SingleInstance();
       
        builder.RegisterType<AppRoleManager>().As<IRoleService>().SingleInstance();

        builder.RegisterType<AuthManager>().As<IAuthService>().SingleInstance();


        builder.Register(context =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<BaseDbContext>();
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("MssqlServerConnection"));
            return new BaseDbContext(optionsBuilder.Options, _configuration);
        }).AsSelf().InstancePerLifetimeScope();

        //çalışan uygulamada interceptor varmı bak 
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();

        builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()
            .EnableInterfaceInterceptors(new ProxyGenerationOptions()
            {
                Selector = new AspectInterceptorSelector()
            }).SingleInstance();
    }
}
