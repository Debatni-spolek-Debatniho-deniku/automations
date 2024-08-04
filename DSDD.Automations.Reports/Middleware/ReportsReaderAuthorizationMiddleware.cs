using DSDD.Automations.Hosting.Middleware;

namespace DSDD.Automations.Reports.Middleware;

public class ReportsReaderAuthorizationMiddleware: AccessRoleAuthorizationMiddlewareBase
{
    public ReportsReaderAuthorizationMiddleware() : base(ROLE_NAME)
    {
    }

    private const string ROLE_NAME = "reports-reader";
}