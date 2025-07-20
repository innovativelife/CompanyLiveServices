namespace InnovativeLife.Common;

public class GcpConstants
{
    public static string ProjectId
    {
        get
        {
            var projectId = Environment.GetEnvironmentVariable("PROJECT_ID");
            if (projectId == null)
            {
                return "development";
            }

            return projectId;
        }
    }

    public static string RootTenantId = "Root";

    public static string RootIdentityManagerTenantId
    {
        get
        {
            var tenantId = Environment.GetEnvironmentVariable("TENANT_ID");
            if (tenantId == null)
            {
                return "development";
            }

            return tenantId;
        }
    }
}