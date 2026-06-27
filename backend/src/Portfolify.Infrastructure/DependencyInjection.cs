using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portfolify.Application.Common.Interfaces;
using Portfolify.Application.Features.Auth.Commands.Register;
using Portfolify.Application.Features.Auth.Commands.Login;
using Portfolify.Domain.Interfaces;
using Portfolify.Infrastructure.Persistence;
using Portfolify.Infrastructure.Repositories;
using Portfolify.Infrastructure.Services;

namespace Portfolify.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<RegisterCommandValidator>();
        services.AddScoped<LoginCommandValidator>();

        return services;
    }
}
