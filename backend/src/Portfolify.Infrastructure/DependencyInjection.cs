using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portfolify.Application.Common.Interfaces;
using Portfolify.Application.Features.Auth.Commands.Register;
using Portfolify.Application.Features.Auth.Commands.Login;
using Portfolify.Application.Features.Auth.Commands.RefreshToken;
using Portfolify.Application.Features.Auth.Commands.Logout;
using Portfolify.Application.Features.Skills.Commands.AddSkill;
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
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<RegisterCommandValidator>();
        services.AddScoped<LoginCommandValidator>();
        services.AddScoped<RefreshTokenCommandValidator>();
        services.AddScoped<LogoutCommandValidator>();
        services.AddScoped<AddSkillCommandValidator>();

        return services;
    }
}
