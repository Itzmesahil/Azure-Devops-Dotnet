namespace DotNetDevOpsDemo.Models;

public class Visitor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime VisitedAtUtc { get; set; }
}
