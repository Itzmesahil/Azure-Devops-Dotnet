var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/health", () => Results.Ok(new {
    status = "healthy",
    application = "DotNet DevOps Demo",
    version = "1.0.0"
}));

app.MapGet("/api/info", () => Results.Ok(new {
    application = "DotNet DevOps Demo",
    version = "1.0.0",
    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
    runtime = Environment.Version.ToString()
}));

app.MapFallbackToFile("index.html");
app.Run();

public partial class Program { }
