using System;
using System.Collections.Generic;
using System.Text;

namespace Fraso.Hal.Primitives
{
    public class Link
    {
        #region Properties      
        public string Title { get; set; }

        public string HRef { get; set; }

        public bool? Templated { get; set; }
        #endregion // Properties

        #region Ctors
        public Link(string hRef)
            => HRef = 
                hRef ?? throw new ArgumentException(nameof(hRef));
        
        #endregion // Ctors
    }
}
