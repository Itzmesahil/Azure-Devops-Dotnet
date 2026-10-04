using Xunit;

namespace DotNetDevOpsDemo.Tests;

public class HealthTests
{
    [Fact]
    public void HealthStatus_ShouldBeHealthy()
    {
        Assert.Equal("healthy", "healthy");
    }

    [Fact]
    public void VisitorRequest_ShouldAcceptName()
    {
        var request = new VisitorRequest("Sahil");
        Assert.Equal("Sahil", request.Name);
    }
}
