using System;
using System.Collections.Generic;

namespace Fraso.Hal.Conversions
{
    public sealed class CollectionWrapPolicy<TCollection, TContent>
        where TCollection : IEnumerable<TContent>
    {
        public WrapPolicy<TCollection> CollectionPolicy;

        public readonly WrapPolicy<TContent> ContentPolicy;

        public CollectionWrapPolicy(
            WrapPolicy<TCollection> collectionPolicy,
            WrapPolicy<TContent> contentPolicy)
        {
            CollectionPolicy =
                collectionPolicy
                    ?? throw new ArgumentNullException(nameof(collectionPolicy));

            ContentPolicy =
                contentPolicy
                    ?? throw new ArgumentNullException(nameof(contentPolicy));
        }
    }
}
