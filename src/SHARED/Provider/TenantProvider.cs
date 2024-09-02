using Microsoft.AspNetCore.Http;

namespace SHARED.Provider;

public class TenantProvider : ITenantProvider
{
    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext == null) return;

        string tenant = httpContextAccessor.HttpContext?.Request.Headers["App-Name"];
        
        if (string.IsNullOrEmpty(tenant) || tenant == "dev" || tenant == "staging" || tenant == "demo")
        {
            tenant = "Entrance";
        }

        Tenant = tenant;
    }

    public string Tenant { get; set; } 
}