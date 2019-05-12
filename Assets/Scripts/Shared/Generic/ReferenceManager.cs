using System.Collections.Generic;

namespace Core
{
    public abstract class ReferenceManager<TRefTo, TRefFrom> where TRefTo : class where TRefFrom : ReferenceManager<TRefTo, TRefFrom>
    {
        private readonly LinkedList<Reference<TRefTo, TRefFrom>> referenceList = new LinkedList<Reference<TRefTo, TRefFrom>>();

        public Reference<TRefTo, TRefFrom> FirstReference => referenceList.First?.Value;
        public Reference<TRefTo, TRefFrom> LastReference => referenceList.Last?.Value;

        public LinkedListNode<Reference<TRefTo, TRefFrom>> Add(Reference<TRefTo, TRefFrom> reference)
        {
            return referenceList.AddFirst(reference);
        }

        public void Clear()
        {
            var node = referenceList.First;

            while (node != null)
            {
                var next = node.Next;
                node.Value.Invalidate();
                node = next;
            }
        }

        public bool Contains(Reference<TRefTo, TRefFrom> reference)
        {
            return referenceList.Contains(reference);
        }
    }
}