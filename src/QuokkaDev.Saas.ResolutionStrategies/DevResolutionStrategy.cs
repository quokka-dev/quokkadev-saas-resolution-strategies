using QuokkaDev.Saas.Abstractions;

namespace QuokkaDev.Saas.ResolutionStrategies
{
    /// <summary>
    /// Resolve tenant identifier with a default value. Used for development purposes
    /// </summary>
    public class DevResolutionStrategy : ITenantResolutionStrategy
    {
        private readonly string tenantIdentifier;

        public DevResolutionStrategy(string tenantIdentifier)
        {
            this.tenantIdentifier = tenantIdentifier;
        }

        public string GetTenantIdentifier()
        {
            return this.tenantIdentifier;
        }

        public Task<string> GetTenantIdentifierAsync()
        {
            return Task.FromResult(GetTenantIdentifier());
        }
    }
}
