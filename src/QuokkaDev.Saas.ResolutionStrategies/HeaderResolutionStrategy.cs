using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using QuokkaDev.Saas.Abstractions;

namespace QuokkaDev.Saas.ResolutionStrategies
{
    /// <summary>
    /// Resolve thetenant identifier using an HTTP Header
    /// </summary>
    public class HeaderResolutionStrategy : ITenantResolutionStrategy
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HeaderResolutionStrategySettings _settings;

        public HeaderResolutionStrategy(IHttpContextAccessor httpContextAccessor, IOptions<HeaderResolutionStrategySettings> settings)
        {
            _httpContextAccessor = httpContextAccessor;
            this._settings = settings.Value;
        }

        public string GetTenantIdentifier()
        {
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(_settings.HeaderName, out StringValues identifierHeaderValues))
            {
                return identifierHeaderValues[0];
            }
            else
            {
                throw new ArgumentException($"Header {_settings.HeaderName} cannot be found in the request");
            }
        }

        public Task<string> GetTenantIdentifierAsync()
        {
            return Task.FromResult(GetTenantIdentifier());
        }
    }

    public class HeaderResolutionStrategySettings
    {
        public string HeaderName { get; set; } = "X-TenantIdentifier";
    }
}
