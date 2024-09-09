namespace InnovativeLife.Localization;

// Could not get resources to work despite much research.
// Used DI, etc, etc.  Never found the resource file.  
// Super simple implementation for now - it is centralised so can be fixed later
public class MessageService : IMessageService
{
    public const string Employee_UID_Mandatory = "Employee_UID_Mandatory";
    public const string Employee_Number_Mandatory = "Employee_Number_Mandatory";
    public const string Email_Address_Mandatory = "Email_Address_Mandatory";   

    private readonly Dictionary<string, string> _messages = new Dictionary<string, string>();

    public MessageService()
    {
        for(int i = 0; i < Messages_en.Messages.GetLength(0); i++)
        {
            _messages.Add(Messages_en.Messages[i, 0], Messages_en.Messages[i, 1]);
        }
    }

    public string GetMessage(string code)
    {
        switch (code)
        {
            case Employee_UID_Mandatory:
                return "Employee UID must be provided";
            case Employee_Number_Mandatory:
                return "Employee Number must be provided";
            case Email_Address_Mandatory:
                return "Email Address must be provided";
        }

        if (!_messages.ContainsKey(code))
        {
            return "Invalid Message Code";
        }

        return _messages[code];
    }

    
}