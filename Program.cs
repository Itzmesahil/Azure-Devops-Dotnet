var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/", () => Results.Ok(new { application = "DotNet DevOps Demo", version = "1.0.0", message = "Application is running successfully" }));
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.Run();
public partial class Program { }
