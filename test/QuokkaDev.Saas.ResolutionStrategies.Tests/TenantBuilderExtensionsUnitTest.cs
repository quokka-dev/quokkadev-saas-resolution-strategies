using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using QuokkaDev.Saas.Abstractions;
using QuokkaDev.Saas.DependencyInjection;
using System.Linq;
using Xunit;

namespace QuokkaDev.Saas.ResolutionStrategies.Tests
{
    public class TenantBuilderExtensionsUnitTest
    {
        public TenantBuilderExtensionsUnitTest()
        {
        }

        [Fact(DisplayName = "DevResolutionStrategy should be used")]
        public void DevResolutionStrategy_Should_Be_Used()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            services.AddMultiTenancy().WithDevResolutionStrategy("my-tenant-identifier");
            var strategy = services.FirstOrDefault(sd => sd.ServiceType == typeof(ITenantResolutionStrategy));

            // Assert
            strategy.Should().NotBeNull();
            strategy?.ImplementationType.Should().BeNull();
            strategy?.Lifetime.Should().Be(ServiceLifetime.Singleton);
            strategy?.ImplementationInstance.Should().NotBeNull();
            strategy?.ImplementationInstance.Should().BeOfType<DevResolutionStrategy>();
        }

        [Fact(DisplayName = "HeaderResolutionStrategy should be used")]
        public void HeaderResolutionStrategy_Should_Be_Used()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            services.AddMultiTenancy().WithHeaderResolutionStrategy(opts => opts.HeaderName = "X-Custom-Header-Name");
            var strategy = services.FirstOrDefault(sd => sd.ServiceType == typeof(ITenantResolutionStrategy));
            var settings = services.FirstOrDefault(sd => sd.ServiceType == typeof(HeaderResolutionStrategySettings));
            var settingsInstance = settings?.ImplementationInstance as HeaderResolutionStrategySettings;
            var httpContextAccessor = services.FirstOrDefault(sd => sd.ServiceType == typeof(IHttpContextAccessor));

            // Assert
            strategy.Should().NotBeNull();
            strategy?.ImplementationType.Should().NotBeNull();
            strategy?.ImplementationType.Should().Be(typeof(HeaderResolutionStrategy));
            strategy?.Lifetime.Should().Be(ServiceLifetime.Transient);
            strategy?.ImplementationInstance.Should().BeNull();

            settings.Should().NotBeNull();
            settings?.ImplementationType.Should().BeNull();
            settings?.Lifetime.Should().Be(ServiceLifetime.Singleton);
            settings?.ImplementationInstance.Should().NotBeNull();
            settings?.ImplementationInstance.Should().BeOfType<HeaderResolutionStrategySettings>();

            settingsInstance?.HeaderName.Should().Be("X-Custom-Header-Name");

            httpContextAccessor.Should().NotBeNull();
        }

        [Fact(DisplayName = "HeaderResolutionStrategy should be use default settings")]
        public void HeaderResolutionStrategy_Should_Be_Use_Default_Settings()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            services.AddMultiTenancy().WithHeaderResolutionStrategy();
            var settings = services.FirstOrDefault(sd => sd.ServiceType == typeof(HeaderResolutionStrategySettings));
            var settingsInstance = settings?.ImplementationInstance as HeaderResolutionStrategySettings;

            // Assert
            settings.Should().NotBeNull();
            settings?.ImplementationType.Should().BeNull();
            settings?.Lifetime.Should().Be(ServiceLifetime.Singleton);
            settings?.ImplementationInstance.Should().NotBeNull();
            settings?.ImplementationInstance.Should().BeOfType<HeaderResolutionStrategySettings>();

            settingsInstance?.HeaderName.Should().Be("X-TenantIdentifier");
        }

        [Fact(DisplayName = "HostResolutionStrategy should be used")]
        public void HostResolutionStrategy_Should_Be_Used()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            services.AddMultiTenancy().WithHostResolutionStrategy();
            var strategy = services.FirstOrDefault(sd => sd.ServiceType == typeof(ITenantResolutionStrategy));
            var httpContextAccessor = services.FirstOrDefault(sd => sd.ServiceType == typeof(IHttpContextAccessor));

            // Assert
            strategy.Should().NotBeNull();
            strategy?.ImplementationType.Should().NotBeNull();
            strategy?.ImplementationType.Should().Be(typeof(HostResolutionStrategy));
            strategy?.Lifetime.Should().Be(ServiceLifetime.Transient);
            strategy?.ImplementationInstance.Should().BeNull();

            httpContextAccessor.Should().NotBeNull();
        }

        [Fact(DisplayName = "PathResolutionStrategy should be used")]
        public void PathResolutionStrategy_Should_Be_Used()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            services.AddMultiTenancy().WithPathResolutionStrategy();
            var strategy = services.FirstOrDefault(sd => sd.ServiceType == typeof(ITenantResolutionStrategy));
            var httpContextAccessor = services.FirstOrDefault(sd => sd.ServiceType == typeof(IHttpContextAccessor));

            // Assert
            strategy.Should().NotBeNull();
            strategy?.ImplementationType.Should().NotBeNull();
            strategy?.ImplementationType.Should().Be(typeof(PathResolutionStrategy));
            strategy?.Lifetime.Should().Be(ServiceLifetime.Transient);
            strategy?.ImplementationInstance.Should().BeNull();

            httpContextAccessor.Should().NotBeNull();
        }
    }
}
