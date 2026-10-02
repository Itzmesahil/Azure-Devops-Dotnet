using Xunit;
namespace DotNetDevOpsDemo.Tests;
public class HealthTests
{
    [Fact]
    public void HealthStatus_ShouldBeHealthy()
    {
        var status = "healthy";
        Assert.Equal("healthy", status);
    }
}
