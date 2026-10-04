using DotNetDevOpsDemo.Data;
using DotNetDevOpsDemo.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var server = Environment.GetEnvironmentVariable("MYSQL_SERVER");
var database = Environment.GetEnvironmentVariable("MYSQL_DATABASE");
var username = Environment.GetEnvironmentVariable("MYSQL_USERNAME");
var password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

if (string.IsNullOrWhiteSpace(server) ||
    string.IsNullOrWhiteSpace(database) ||
    string.IsNullOrWhiteSpace(username) ||
    string.IsNullOrWhiteSpace(password))
{
    throw new InvalidOperationException("Required MySQL environment variables are missing.");
}

var connectionString =
    $"Server={server};Port=3306;Database={database};User={username};Password={password};SslMode=Required;";

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
    version = "3.0.0"
}));

app.MapGet("/api/info", () => Results.Ok(new
{
    application = "DotNet DevOps Demo",
    version = "3.0.0",
    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
    runtime = Environment.Version.ToString()
}));

app.MapGet("/api/visitors/stats", async (AppDbContext db) =>
{
    try
    {
        var todayUtc = DateTime.UtcNow.Date;
        var total = await db.Visitors.CountAsync();
        var today = await db.Visitors.CountAsync(v => v.VisitedAtUtc >= todayUtc);
        var latest = await db.Visitors
            .AsNoTracking()
            .OrderByDescending(v => v.VisitedAtUtc)
            .Select(v => new { v.Name, v.VisitedAtUtc })
            .FirstOrDefaultAsync();

        return Results.Ok(new
        {
            total,
            today,
            latestVisitor = latest?.Name,
            latestVisitUtc = latest?.VisitedAtUtc
        });
    }
    catch
    {
        return Results.Problem(
            title: "Visitor statistics unavailable",
            detail: "The database could not be reached right now.",
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
});

app.MapPost("/api/visitors", async (VisitorRequest request, AppDbContext db) =>
{
    var name = request.Name?.Trim();

    if (string.IsNullOrWhiteSpace(name))
        return Results.BadRequest(new { message = "Name is required." });

    if (name.Length > 100)
        return Results.BadRequest(new { message = "Name must be 100 characters or fewer." });

    try
    {
        var visitor = new Visitor
        {
            Name = name,
            VisitedAtUtc = DateTime.UtcNow
        };

        db.Visitors.Add(visitor);
        await db.SaveChangesAsync();

        return Results.Created($"/api/visitors/{visitor.Id}", visitor);
    }
    catch
    {
        return Results.Problem(
            title: "Visit could not be recorded",
            detail: "The database could not save the visitor right now.",
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
});

app.MapGet("/api/visitors", async (AppDbContext db) =>
{
    try
    {
        var visitors = await db.Visitors
            .AsNoTracking()
            .OrderByDescending(v => v.VisitedAtUtc)
            .Take(100)
            .ToListAsync();

        return Results.Ok(visitors);
    }
    catch
    {
        return Results.Problem(
            title: "Visitor list unavailable",
            detail: "The database could not be reached right now.",
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
});

app.MapFallbackToFile("index.html");
app.Run();

public record VisitorRequest(string? Name);
public partial class Program { }
