using System.Collections.Generic;

namespace Fraso.Hal.Primitives
{
    public interface IResource
    {
        IReadOnlyCollection<NamedLink> Links { get; }

        IReadOnlyCollection<IResource> Embedded { get; }

        IEnumerable<string> Properties { get; }

        object this[string propertyName] { get; }
    }
}
