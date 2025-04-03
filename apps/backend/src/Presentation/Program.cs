using Presentation;
using Presentation.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGenWithAuth("Jala Match Api");

builder
    .Services
    .AddPresentation(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();
}

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

await app.RunAsync();
