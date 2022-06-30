using Microsoft.AspNetCore.Http;
using QuokkaDev.Saas.Abstractions;

namespace QuokkaDev.Saas.ResolutionStrategies
{
    /// <summary>
    /// Resolve the host to a tenant identifier
    /// </summary>
    public class HostResolutionStrategy : ITenantResolutionStrategy
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HostResolutionStrategy(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetTenantIdentifier()
        {
            return _httpContextAccessor.HttpContext.Request.Host.Host;
        }

        public Task<string> GetTenantIdentifierAsync()
        {
            return Task.FromResult(GetTenantIdentifier());
        }
    }
}
