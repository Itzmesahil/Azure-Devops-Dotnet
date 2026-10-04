using DotNetDevOpsDemo.Data;
using DotNetDevOpsDemo.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Read MySQL configuration from environment variables
var server = Environment.GetEnvironmentVariable("MYSQL_SERVER");
var database = Environment.GetEnvironmentVariable("MYSQL_DATABASE");
var username = Environment.GetEnvironmentVariable("MYSQL_USERNAME");
var password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

// Build MySQL connection string
var connectionString =
    $"Server={server};Port=3306;Database={database};User={username};Password={password};SslMode=Required;";

// Configure Entity Framework Core with MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 4, 0))));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "healthy",
    application = "DotNet DevOps Demo",
    version = "2.0.0"
}));

app.MapGet("/api/info", () => Results.Ok(new
{
    application = "DotNet DevOps Demo",
    version = "2.0.0",
    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
    runtime = Environment.Version.ToString()
}));

app.MapPost("/api/visitors", async (VisitorRequest request, AppDbContext db) =>
{
    var name = request.Name?.Trim();

    if (string.IsNullOrWhiteSpace(name))
        return Results.BadRequest(new { message = "Name is required." });

    if (name.Length > 100)
        return Results.BadRequest(new { message = "Name must be 100 characters or fewer." });

    var visitor = new Visitor
    {
        Name = name,
        VisitedAtUtc = DateTime.UtcNow
    };

    db.Visitors.Add(visitor);
    await db.SaveChangesAsync();

    return Results.Created($"/api/visitors/{visitor.Id}", visitor);
});

app.MapGet("/api/visitors", async (AppDbContext db) =>
{
    var visitors = await db.Visitors
        .AsNoTracking()
        .OrderByDescending(v => v.VisitedAtUtc)
        .Take(100)
        .ToListAsync();

    return Results.Ok(visitors);
});

app.MapFallbackToFile("index.html");

app.Run();

public record VisitorRequest(string? Name);

public partial class Program { }