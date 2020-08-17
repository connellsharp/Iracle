using System.Collections.Generic;

namespace Iracle
{
    public class IrcSettings : IIrcLoginSettings, ITcpConnectionSettings
    {
        public string Password { get; set; }

        public string Nick { get; set; }

        public string User { get; set; }

        public IEnumerable<string> Channels { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }
    }
}