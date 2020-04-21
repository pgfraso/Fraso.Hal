using System.Collections.Generic;

namespace Fraso.Hal.Primitives
{
    public sealed class Resource
        : IResource
    {
        #region Fields
        private readonly List<NamedLink> _links
            = new List<NamedLink>();

        private readonly List<IResource> _embedded
            = new List<IResource>();

        private readonly FieldsValues _properties
            = new FieldsValues();
        #endregion // Fields

        #region Properties
        public IReadOnlyCollection<NamedLink> Links => _links;

        public IReadOnlyCollection<IResource> Embedded => _embedded;

        public IEnumerable<string> Properties => _properties.Keys;

        public object this[string propertyName]
        {
            get => _properties[propertyName];
            set => _properties[propertyName] = value;
        }
        #endregion // Properties

        public void Link(NamedLink link)
            => _links.Add(link);

        public void Link(IEnumerable<NamedLink> links)
            => _links.AddRange(links);

        public void Embed(Resource resource)
            => _embedded.Add(resource);
    }
}
