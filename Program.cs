using DotNetDevOpsDemo.Data;
using DotNetDevOpsDemo.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));
}

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
