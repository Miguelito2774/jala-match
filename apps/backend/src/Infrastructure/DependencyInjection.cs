using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;
using Application.Abstractions.Data;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Services;
using Infrastructure.Authentication;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
            .AddServices(configuration)
            .AddDatabase(configuration)
            .AddHealthChecks(configuration)
            .AddAuthenticationInternal(configuration)
            .AddAuthorizationInternal()
            .AddCacheInternal(configuration)
            .AddEmailServices(configuration);
    }

    private static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<ISfiaCalculatorService, SfiaCalculatorService>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IEmployeeProfileRepository, EmployeeProfileRepository>();
        services.AddScoped<ITechnologyRepository, TechnologyRepository>();
        services.AddScoped<IUserPrivacyConsentRepository, UserPrivacyConsentRepository>();
        services.AddScoped<IDataDeletionRequestRepository, DataDeletionRequestRepository>();
        services.AddScoped<IPrivacyAuditLogRepository, PrivacyAuditLogRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<IImageStorageService, ImageStorageService>();

        // Background notification service
        services.AddSingleton<IBackgroundNotificationQueue, BackgroundNotificationService>();
        services.AddHostedService<BackgroundNotificationService>(provider => 
            (BackgroundNotificationService)provider.GetRequiredService<IBackgroundNotificationQueue>());

        services.AddHttpClient(
            "AIService",
            client =>
            {
                client.BaseAddress = new Uri(configuration["AIService:BaseUrl"]!);
                client.Timeout = TimeSpan.FromMinutes(5);
            }
        );

        // Configurar Cloudinary igual que Email - primero variables de entorno, luego appsettings
        string cloudinaryCloudName =
            Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME")
            ?? configuration["Cloudinary:CloudName"];
        string cloudinaryApiKey =
            Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY")
            ?? configuration["Cloudinary:ApiKey"];
        string cloudinaryApiSecret =
            Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
            ?? configuration["Cloudinary:ApiSecret"];

        // Solo configurar si tenemos las credenciales necesarias
        if (!string.IsNullOrEmpty(cloudinaryCloudName)
            && !string.IsNullOrEmpty(cloudinaryApiKey)
            && !string.IsNullOrEmpty(cloudinaryApiSecret)
            && !cloudinaryCloudName.StartsWith("${", StringComparison.Ordinal)
            && !cloudinaryApiKey.StartsWith("${", StringComparison.Ordinal)
            && !cloudinaryApiSecret.StartsWith("${", StringComparison.Ordinal))
        {
            services
                .AddOptions<CloudinarySettings>()
                .Configure(opts =>
                {
                    opts.CloudName = cloudinaryCloudName;
                    opts.ApiKey = cloudinaryApiKey;
                    opts.ApiSecret = cloudinaryApiSecret;
                })
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
        else
        {
            // Configurar con valores vacíos si no están disponibles (para desarrollo)
            services
                .AddOptions<CloudinarySettings>()
                .Configure(opts =>
                {
                    opts.CloudName = string.Empty;
                    opts.ApiKey = string.Empty;
                    opts.ApiSecret = string.Empty;
                });
        }

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

    private static IServiceCollection AddAuthenticationInternal(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        JwtSettings? jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings?.Issuer,
                    ValidAudience = jwtSettings?.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings?.SecretKey!)
                    ),
                    ClockSkew = TimeSpan.Zero,
                }
            );

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

    private static IServiceCollection AddEmailServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string senderEmail =
            Environment.GetEnvironmentVariable("EMAIL_SENDER_EMAIL")
            ?? configuration["Email:SenderEmail"]
            ?? "noreply@jalamatch.com";
        string senderName =
            Environment.GetEnvironmentVariable("EMAIL_SENDER_NAME")
            ?? configuration["Email:SenderName"]
            ?? "Jala Match";

        string smtpHost =
            Environment.GetEnvironmentVariable("EMAIL_SMTP_HOST")
            ?? configuration["Email:Smtp:Host"];
        string smtpUsername =
            Environment.GetEnvironmentVariable("EMAIL_SMTP_USERNAME")
            ?? configuration["Email:Smtp:Username"];
        string smtpPassword =
            Environment.GetEnvironmentVariable("EMAIL_SMTP_PASSWORD")
            ?? configuration["Email:Smtp:Password"];

        if (
            !string.IsNullOrEmpty(smtpHost)
            && !string.IsNullOrEmpty(smtpUsername)
            && !string.IsNullOrEmpty(smtpPassword)
            && !smtpHost.StartsWith("${", StringComparison.Ordinal)
            && !smtpUsername.StartsWith("${", StringComparison.Ordinal)
            && !smtpPassword.StartsWith("${", StringComparison.Ordinal)
        )
        {
            string portString =
                Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT")
                ?? configuration["Email:Smtp:Port"]
                ?? "587";
            string sslString =
                Environment.GetEnvironmentVariable("EMAIL_SMTP_ENABLE_SSL")
                ?? configuration["Email:Smtp:EnableSsl"]
                ?? "true";

            if (portString.StartsWith("${", StringComparison.Ordinal))
            {
                portString = "587";
            }

            if (sslString.StartsWith("${", StringComparison.Ordinal))
            {
                sslString = "true";
            }

            int smtpPort = int.Parse(portString, CultureInfo.InvariantCulture);
            bool enableSsl = bool.Parse(sslString);
            int smtpTimeout = configuration.GetValue<int>("Email:SmtpTimeoutMs", 30000);

            services
                .AddFluentEmail(senderEmail, senderName)
                .AddSmtpSender(
                    new SmtpClient(smtpHost)
                    {
                        Port = smtpPort,
                        Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                        EnableSsl = enableSsl,
                        Timeout = smtpTimeout,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                    }
                );
        }
        else
        {
            // Configure FluentEmail with a mock sender for development
            services.AddFluentEmail(senderEmail, senderName);
        }

        return services;
    }
}
