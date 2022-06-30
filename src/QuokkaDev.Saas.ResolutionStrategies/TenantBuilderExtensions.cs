using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QuokkaDev.Saas.Abstractions;
using QuokkaDev.Saas.ResolutionStrategies;

namespace QuokkaDev.Saas.DependencyInjection
{
    public static class TenantBuilderExtensions
    {
        public static TenantBuilder<T, TKey> WithDevResolutionStrategy<T, TKey>(this TenantBuilder<T, TKey> builder, string tenantIdentifier) where T : Tenant<TKey>
        {
            builder.WithResolutionStrategy<DevResolutionStrategy>(new DevResolutionStrategy(tenantIdentifier));
            return builder;
        }

        public static TenantBuilder<T, TKey> WithHostResolutionStrategy<T, TKey>(this TenantBuilder<T, TKey> builder) where T : Tenant<TKey>
        {
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.WithResolutionStrategy<HostResolutionStrategy>();
            return builder;
        }

        public static TenantBuilder<T, TKey> WithPathResolutionStrategy<T, TKey>(this TenantBuilder<T, TKey> builder) where T : Tenant<TKey>
        {
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.WithResolutionStrategy<PathResolutionStrategy>();
            return builder;
        }

        public static TenantBuilder<T, TKey> WithHeaderResolutionStrategy<T, TKey>(this TenantBuilder<T, TKey> builder, Action<HeaderResolutionStrategySettings>? configureSettings = null) where T : Tenant<TKey>
        {
            configureSettings ??= ((HeaderResolutionStrategySettings _) => { });
            var settings = new HeaderResolutionStrategySettings();
            configureSettings?.Invoke(settings);

            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.TryAddSingleton<HeaderResolutionStrategySettings>(settings);
            builder.WithResolutionStrategy<HeaderResolutionStrategy>();
            return builder;
        }
    }
}
