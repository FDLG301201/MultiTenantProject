using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Infraestructura.Security;

namespace MultiTenancy.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        public TenantMiddleware(RequestDelegate next) { _next = next; }

        public async Task Invoke(HttpContext context, ITenantProvider tenantProvider)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var claim = context.User.Claims.FirstOrDefault(c => c.Type == "tenantId");
                if (claim != null && Guid.TryParse(claim.Value, out var tenantId))
                {
                    tenantProvider.TenantId = tenantId;
                }
            }
            await _next(context);
        }
    }

}
