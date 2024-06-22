namespace InnovativeLife.Localization;

// Could not get resources to work despite much research.
// Used DI, etc, etc.  Never found the resource file.  
// Super simple implementation for now - it is centralised so can be fixed later
public class MessageService : IMessageService
{
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
        if (!_messages.ContainsKey(code))
        {
            return "Invalid Message Code";
        }

        return _messages[code];
    }

    
}