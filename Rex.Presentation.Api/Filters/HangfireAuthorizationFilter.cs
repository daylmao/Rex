using Hangfire.Dashboard;
using Rex.Enum;

namespace Rex.Presentation.Api.Filters;

public class HangfireAuthorizationFilter(string environment) : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        if (httpContext.Connection.RemoteIpAddress?.Equals(httpContext.Connection.LocalIpAddress) == true ||
            httpContext.Request.Host.Host.Contains("localhost"))
        {
            return true;
        }

        if (environment == "Development")
        {
            return true;
        }

        return httpContext.User.IsInRole(UserRole.Admin.ToString());
    }
}
