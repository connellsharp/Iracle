namespace Iracle
{
    public class Identity
    {
        public Identity(string userString)
        {
            var split = userString.Split('!', '@');

            Nick = split[0];
            User = split[1];
            Host = split[2];
        }

        public string Nick { get; }

        public string User { get; }

        public string Host { get; }
    }
}