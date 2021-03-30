using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Input
{
    /// <summary>
    /// Input does nothing but return the same result as before
    /// </summary>
    public class NullCommandPressReleaseTranslator : IInputTranslator<IEnumerable<string>, IEnumerable<string>>
    {
        public IEnumerable<string> Translate(IEnumerable<string> input) => input;
        public NullCommandPressReleaseTranslator(){
            
        }
    }
}
