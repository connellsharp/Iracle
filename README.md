# Iracle

Iracle provides an easy way to create IRC bots in C#.

### Just write your bot class

```csharp
internal class SlapBot : IBot
{
    public async Task HandleAsync(BotCommand command)
    {
        if(command.Message.StartsWith("!slap "))
        {
            var user = command.Substring(6);
            
            EventHappened.Invoke(new BotEvent
            {
                Channel = command.Channel,
                Message = $"{command.From.User} slaps {user} around a bit with a large trout"
            });
        }
    }

    public event Action<BotEvent> EventHappened;
}
```

Or a simple responder.

```csharp
internal class SlapResponder : IResponder
{
    public async Task<string> HandleAsync(BotCommand command)
    {
        if(command.Message.StartsWith("!slap "))
        {
            var user = command.Message.Substring(6);
            return $"{command.From.User} slaps {user} around a bit with a large trout";
        }

        return null;
    }
}
```

### And connect it to IRC

```csharp
var settings = new IrcSettings 
{
    Host = "irc.example.com",
    Port = 6667,
    Password = "password",
    Nick = "SlapBot",
    User = "SlapBot", 
    Channels = new[] { "#general" }
};

var connection = new IrcBotConnection(settings);
connection.AddBot(new ResponderBot(new SlapResponder()));
await connection.ConnectAsync();
```