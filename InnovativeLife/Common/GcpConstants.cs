namespace InnovativeLife.Common;

public class GcpConstants
{
    public static string ProjectId = "companylive-c3879";
    public static string RootTenantId = "Root";
    public static string RootIdentityManagerTenantId
    {
        get
        {
            // Determine if executing in development mode
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            var devMode = env != null && env.ToLower() == "development";
            if (devMode)
            {
                return "Root";
            }
            var staging = env != null && env.ToLower() == "staging";
            if (staging)
            {
                return "Root-8x3y3";
            }

            // production
            return "Root-0ewfy";
        }
    }

}
