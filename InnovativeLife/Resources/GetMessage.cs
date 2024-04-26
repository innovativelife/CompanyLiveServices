using System;
using System.Reflection;
using System.Resources;

namespace InnovativeLife.Resources;

public static class Message
{
    private static ResourceManager? resourceManager = null;
    private static ResourceManager Resource()
    {
        if (resourceManager == null)
        {
            resourceManager = new ResourceManager("Messages", typeof(Message).Assembly);
        }

        return resourceManager;
    }
    public static string Get(string Id)
    {
        var result = Resource().GetString(Id);
        return result ?? throw new ApplicationException("Invalid Id provided for message");
    }
}