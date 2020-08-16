using System.Collections.Generic;

namespace Iracle
{
    public class IrcConnectionSettings
    {
        public string Password { get; set; }
        public string Nick { get; set; }
        public string User { get; set; }
        public IEnumerable<string> Channels { get; set; }
    }
}