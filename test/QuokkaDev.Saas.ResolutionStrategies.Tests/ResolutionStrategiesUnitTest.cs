using FluentAssertions;
using HttpContextMoq;
using HttpContextMoq.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using QuokkaDev.Saas.Abstractions;
using System;
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

    [Fact(DisplayName = "HeaderResolutionStrategy throw exception if header not found")]
    public void HeaderResolutionStrategy_Throw_Exception_If_Header_Not_Found()
    {
        // Arrange
        HeaderResolutionStrategySettings settings = new() { HeaderName = "X-wrong-header" };
        var settingsMock = new Mock<IOptions<HeaderResolutionStrategySettings>>();
        settingsMock.Setup(m => m.Value).Returns(settings);

        HeaderResolutionStrategy strategy = new(SetupIHttpContextAccessor(), settingsMock.Object);

        // Act        
        Func<string> strategyInvocation = () => strategy.GetTenantIdentifier();

        // Assert

        strategyInvocation.Should().Throw<ArgumentException>().WithMessage("Header X-wrong-header cannot be found in the request");
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

    [Fact(DisplayName = "PathResolutionStrategy throw exception if tenant not found")]
    public void PathResolutionStrategy_Throw_Exception_If_Tenant_Not_Found()
    {
        // Arrange
        PathResolutionStrategy strategy = new(SetupIHttpContextAccessor("https://my-tenant-identifier"));

        // Act        
        Func<string> strategyInvocation = () => strategy.GetTenantIdentifier();

        // Assert
        strategyInvocation.Should().Throw<ArgumentException>().WithMessage("Cannot be found a valid identifier in request path*");
    }

    private static IHttpContextAccessor SetupIHttpContextAccessor(string url = "https://my-tenant-identifier/my-tenant-identifier/api/my-api-call")
    {
        var context = new HttpContextMock();
        context.SetupRequestHeaders(new Dictionary<string, StringValues>()
        {
            { "X-TenantIdentifier", new StringValues("my-tenant-identifier") }
        });
        context.SetupUrl(url);

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(m => m.HttpContext).Returns(context);

        return httpContextAccessorMock.Object;
    }

    private static async Task AssertStrategy(ITenantResolutionStrategy strategy)
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