using System;
using System.Collections.Generic;
using System.Linq;

namespace States.Core.Common
{
    public class SharedKeyEnumerable<TKey>
    {
        private IEnumerable<TKey> Source { get; set; }
        private static int Position = 0;

        public SharedKeyEnumerable(IEnumerable<TKey> source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public IEnumerable<TKey> Iterate()
        {
            var maxCount = Source.Count();
            for (var currentPos = Position; currentPos < maxCount; currentPos++)
            {
                Position = currentPos;
                Current = Source.ElementAt(currentPos);
                yield return Current;
            }
        }
        public TKey Current { get; private set; }
    }
}