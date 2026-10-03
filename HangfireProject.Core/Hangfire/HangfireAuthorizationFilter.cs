using Hangfire.Dashboard;

namespace HangfireProject.Core.Hangfire;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly string _userName = "admin";
    private readonly string _password = "admin";

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        if (httpContext != null)
        {
            var header = httpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(header) && header.StartsWith("Basic "))
            {
                var encodedCredentials = header.Substring("Basic ".Length).Trim();
                var decodedBytes = Convert.FromBase64String(encodedCredentials);
                var decodedString = System.Text.Encoding.UTF8.GetString(decodedBytes);
                var parts = decodedString.Split(':');
                if (parts.Length == 2)
                {
                    var userName = parts[0];
                    var password = parts[1];
                    if (userName == _userName && password == _password)
                    {
                        return true;
                    }
                }
            }

            httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
            httpContext.Response.StatusCode = 401;
        }
        return false;
    }
}