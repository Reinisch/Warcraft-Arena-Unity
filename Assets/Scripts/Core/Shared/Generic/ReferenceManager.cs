using System.Collections.Generic;

namespace Core
{
    public abstract class ReferenceManager<TRefTo, TRefFrom> : LinkedList<Reference<TRefTo, TRefFrom>> where TRefTo : class where TRefFrom : class
    {
        public Reference<TRefTo, TRefFrom> FirstReference => First?.Value;
        public Reference<TRefTo, TRefFrom> LastReference => Last?.Value;

        public void Invalidate()
        {
            var node = First;

            while (node != null)
            {
                var next = node.Next;
                node.Value.Invalidate();
                node = next;
            }
        }
    }
}