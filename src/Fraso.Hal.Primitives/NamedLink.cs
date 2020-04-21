namespace Fraso.Hal.Primitives
{
    public class NamedLink
    {
        public readonly string Name;
        public readonly Link Link;

        public NamedLink(string name, Link link)
        {
            Name = name;
            Link = link;
        }
    }
}
