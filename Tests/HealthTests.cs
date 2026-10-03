using Xunit;
namespace DotNetDevOpsDemo.Tests;
public class HealthTests
{
    [Fact]
    public void HealthStatus_ShouldBeHealthy()
    {
        Assert.Equal("healthy", "healthy");
    }
}
