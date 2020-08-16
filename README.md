# Iracle

Iracle provides an easy way to create IRC bots in C#.

### Just write your bot class

```csharp
internal class SlapBot : IBot
{
    public async Task<string> InvokeCommandAsync(BotCommandContext context, string command)
    {
        if(command.StartsWith("!slap "))
        {
            var user = command.Substring(6);
            return $"{context.Identity.User} slaps {user} around a bit with a large trout";
        }

        return null;
    }

    public event Action<BotEvent> EventHappened;
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
ircConnection.AddBot(new SlapBot());
await ircConnection.ConnectAsync();
```