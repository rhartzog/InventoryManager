using System;
using System.Collections.Generic;

namespace Decats.Core.Helpers
{
    public sealed class Range<T>
    {
        public T LowerBound { get; private set; }
        public T UpperBound { get; private set; }
        public bool InclusiveLowerBound { get; private set; }
        public bool InclusiveUpperBound { get; private set; }
        public IComparer<T> Comparer { get; private set; }

        public Range(T lowerBound, T upperBound,
                      bool inclusiveLowerBound = true,
                      bool inclusiveUpperBound = false,
                      IComparer<T> comparer = null)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
            InclusiveLowerBound = inclusiveLowerBound;
            InclusiveUpperBound = inclusiveUpperBound;
            Comparer = comparer ?? Comparer<T>.Default;

            if (Comparer.Compare(LowerBound, UpperBound) > 0)
            {
                throw new ArgumentException("Invalid bounds");
            }
        }

        public bool Contains(T item)
        {
            var lowerCompare = Comparer.Compare(LowerBound, item);
            if (lowerCompare > (InclusiveLowerBound ? 0 : -1))
            {
                return false;
            }

            var upperCompare = Comparer.Compare(item, UpperBound);
            if (upperCompare > (InclusiveUpperBound ? 0 : -1))
            {
                return false;
            }

            return true;
        }
    }
}
