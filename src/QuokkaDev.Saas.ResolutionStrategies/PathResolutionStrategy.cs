using Microsoft.AspNetCore.Http;
using QuokkaDev.Saas.Abstractions;

namespace QuokkaDev.Saas.ResolutionStrategies
{
    /// <summary>
    /// Resolve tenant identifier using the first segment of request path
    /// </summary>
    public class PathResolutionStrategy : ITenantResolutionStrategy
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PathResolutionStrategy(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetTenantIdentifier()
        {
            var tokens = _httpContextAccessor.HttpContext.Request.Path.Value.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length > 0)
            {
                return tokens[0];
            }
            else
            {
                throw new ArgumentException($"Cannot be found a valid identifier in request path {_httpContextAccessor.HttpContext.Request.Path}");
            }
        }

        public Task<string> GetTenantIdentifierAsync()
        {
            return Task.FromResult(GetTenantIdentifier());
        }
    }
}
