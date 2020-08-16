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
var communicator = new TcpLineCommunicator("irc.example.com", 6667);

var settings = new IrcConnectionSettings 
{
    Password = "password",
    Nick = "SlapBot",
    User = "SlapBot", 
    Channels = new[] { "#general" }
};

var ircConnection = new IrcConnection(communicator, settings);
ircConnection.AddBot(new ResponderBot(new SlapResponder()));
await ircConnection.ConnectAsync();
```