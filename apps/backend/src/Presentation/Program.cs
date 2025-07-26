using Application;
using Application.Abstractions.Data;
using DotNetEnv;
using HealthChecks.UI.Client;
using Infrastructure;
using Infrastructure.Database.Seeders;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Presentation;
using Presentation.Extensions;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Load .env file - try multiple locations
string[] envPaths = new[]
{
    Path.Combine(Directory.GetCurrentDirectory(), ".env"),
    Path.Combine(AppContext.BaseDirectory, ".env"),
    Path.Combine(AppContext.BaseDirectory, "..", "..", ".env"),
    "/home/miguelito/Desktop/jala-match/apps/backend/.env",
};

foreach (string envPath in envPaths)
{
    if (File.Exists(envPath))
    {
        Console.WriteLine($"Loading .env from: {envPath}");
        Env.Load(envPath);
        break;
    }
    else
    {
        Console.WriteLine($".env not found at: {envPath}");
    }
}

builder.Services.AddCors(options =>
    options.AddPolicy(
        "AllowFrontend",
        policy =>
            policy
                .WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
    )
);

builder.Services.AddSwaggerGenWithAuth("Jala Match Api");

builder.Host.UseSerilog(
    (context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration)
);

builder
    .Services.AddApplication()
    .AddPresentation(builder.Configuration)
    .AddInfrastructure(builder.Configuration, builder.Environment);

WebApplication app = builder.Build();

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();

    await app.ApplyMigrations();

    using IServiceScope scope = app.Services.CreateScope();
    IApplicationDbContext dbContext =
        scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
    await AdminSeeder.SeedAdminUser(dbContext);
}

app.MapHealthChecks(
    "/health",
    new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse }
);

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
