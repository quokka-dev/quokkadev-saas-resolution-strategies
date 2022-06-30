using FluentAssertions;
using HttpContextMoq;
using HttpContextMoq.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using QuokkaDev.Saas.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace QuokkaDev.Saas.ResolutionStrategies.Tests;

public class ResolutionStrategiesUnitTest
{
    [Fact(DisplayName = "DevResolutionStrategy works as expected")]
    public async Task DevResolutionStrategy_Works_As_Expected()
    {
        // Arrange
        DevResolutionStrategy strategy = new("my-tenant-identifier");

        // Act        

        // Assert
        await AssertStrategy(strategy);
    }

    [Fact(DisplayName = "HeaderResolutionStrategy works as expected")]
    public async Task HeaderResolutionStrategy_Works_As_Expected()
    {
        // Arrange
        HeaderResolutionStrategySettings settings = new();
        var settingsMock = new Mock<IOptions<HeaderResolutionStrategySettings>>();
        settingsMock.Setup(m => m.Value).Returns(settings);

        HeaderResolutionStrategy strategy = new(SetupIHttpContextAccessor(), settingsMock.Object);

        // Act        

        // Assert
        await AssertStrategy(strategy);
    }

    [Fact(DisplayName = "HostResolutionStrategy works as expected")]
    public async Task HostResolutionStrategy_Works_As_Expected()
    {
        // Arrange
        HostResolutionStrategy strategy = new(SetupIHttpContextAccessor());

        // Act        

        // Assert
        await AssertStrategy(strategy);
    }

    [Fact(DisplayName = "PathResolutionStrategy works as expected")]
    public async Task PathResolutionStrategy_Works_As_Expected()
    {
        // Arrange
        PathResolutionStrategy strategy = new(SetupIHttpContextAccessor());

        // Act        

        // Assert
        await AssertStrategy(strategy);
    }

    private IHttpContextAccessor SetupIHttpContextAccessor()
    {
        var context = new HttpContextMock();
        context.SetupRequestHeaders(new Dictionary<string, StringValues>()
        {
            { "X-TenantIdentifier", new StringValues("my-tenant-identifier") }
        });
        context.SetupUrl("https://my-tenant-identifier/my-tenant-identifier/api/my-api-call");

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(m => m.HttpContext).Returns(context);

        return httpContextAccessorMock.Object;
    }

    private async Task AssertStrategy(ITenantResolutionStrategy strategy)
    {
        // Act
        string idFromyncMethod = strategy.GetTenantIdentifier();
        string idFromAsyncMethod = await strategy.GetTenantIdentifierAsync();

        idFromyncMethod.Should().NotBeNullOrWhiteSpace();
        idFromyncMethod.Should().Be("my-tenant-identifier");

        idFromAsyncMethod.Should().NotBeNullOrWhiteSpace();
        idFromAsyncMethod.Should().Be("my-tenant-identifier");
    }
}