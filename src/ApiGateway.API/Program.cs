using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("Configuration/ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddOcelot(builder.Configuration).AddPolly();
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/health", () => Results.Json(new { status = "Healthy" }));
});

await app.UseOcelot();

app.Run();

public partial class Program { }
