using System.Collections.Generic;

namespace Iracle
{
    public interface IIrcLoginSettings 
    {
        string Password { get; }
        string Nick { get; }
        string User { get; }
        IEnumerable<string> Channels { get; }
    }

    public class IrcLoginSettings : IIrcLoginSettings
    {
        public string Password { get; set; }
        public string Nick { get; set; }
        public string User { get; set; }
        public IEnumerable<string> Channels { get; set; }
    }
}