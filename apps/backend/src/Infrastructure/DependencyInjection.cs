using Application.Abstractions.Data;
using Domain.Services;
using Infrastructure.Database;
using Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment
    )
    {
        return services
            .AddServices()
            .AddDatabase(configuration)
            .AddHealthChecks(configuration)
            .AddAuthenticationInternal()
            .AddAuthorizationInternal()
            .AddCacheInternal(configuration);
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICacheService, CacheService>();

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string? connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<ApplicationDbContext>(options =>
            options
                .UseNpgsql(
                    connectionString,
                    npgsqlOptions =>
                        npgsqlOptions.MigrationsHistoryTable(
                            HistoryRepository.DefaultTableName,
                            Schemas.Default
                        )
                )
                .UseSnakeCaseNamingConvention()
        );

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>()
        );

        return services;
    }

    private static IServiceCollection AddHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        if (!configuration.GetValue<bool>("HealthChecks:Enabled"))
        {
            return services;
        }

        services
            .AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Database")!)
            .AddRedis(configuration.GetConnectionString("Redis")!);

        return services;
    }

    private static IServiceCollection AddAuthenticationInternal(this IServiceCollection services)
    {
        return services;
    }

    private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
    {
        services.AddAuthorization();

        return services;
    }

    private static IServiceCollection AddCacheInternal(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddFusionCache()
            .WithDefaultEntryOptions(options =>
                options.Duration = TimeSpan.FromMinutes(
                    configuration.GetValue<int>("Cache:DefaultDurationMinutes")
                )
            )
            .WithSerializer(new FusionCacheSystemTextJsonSerializer())
            .WithDistributedCache(
                new RedisCache(
                    new RedisCacheOptions
                    {
                        Configuration = configuration.GetConnectionString("Redis"),
                    }
                )
            )
            .AsHybridCache();

        return services;
    }
}
