using System.Collections.Generic;

namespace Iracle
{
    public class IrcBotSettings
    {
        public string Nick { get; set; }
        public string User { get; set; }
        public IEnumerable<string> Channels { get; set; }
    }
}