using Autofac.Extensions.DependencyInjection;
using Autofac;
using Business.DependencyResolver;
using DataAccess;
using Autofac.Core;
using DataAccess.Context;
using Core.Entities.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Core.Utilities.Security.Encryption;
using Core.Utilities.Security.JWT;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using DataAccess.Security;
using Core.Utilities.IoC;
using Core.Utilities.Exceptions;

var builder = WebApplication.CreateBuilder(args);

#region Identity Options

builder.Services.AddIdentityCore<User>()
                .AddRoles<UserRole>()
                .AddSignInManager()
                .AddEntityFrameworkStores<BaseDbContext>()
                .AddDefaultTokenProviders();
#endregion

#region JWT Options

var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<Core.Utilities.Security.JWT.TokenOptions>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
        };
    });
   ServiceTool.Create(builder.Services);

#endregion

#region AutoFac

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder2 =>
    builder2.RegisterModule(new AutofacBusinessModule(builder.Configuration))
    );

#endregion

builder.Services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region Seed Role and Permission Data

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("app");
    try
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<UserRole>>();
        await Seeds.DefaultRoles.SeedAsync(roleManager);
        await Seeds.DefaultUsers.SeedBasicUserAsync(userManager);
        await Seeds.DefaultUsers.SeedSuperAdminAsync(userManager, roleManager);
        logger.LogInformation("Finished Seeding Default Data");
        logger.LogInformation("Application Starting");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "An error occurred seeding the DB");
    }
}

#endregion

app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
