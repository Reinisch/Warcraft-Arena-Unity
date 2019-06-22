using System.Collections.Generic;

namespace Common
{
    public abstract class ReferenceManager<TRefTo, TRefFrom> where TRefTo : class where TRefFrom : ReferenceManager<TRefTo, TRefFrom>
    {
        protected readonly LinkedList<Reference<TRefTo, TRefFrom>> ReferenceList = new LinkedList<Reference<TRefTo, TRefFrom>>();

        protected Reference<TRefTo, TRefFrom> FirstReference => ReferenceList.First?.Value;

        public LinkedListNode<Reference<TRefTo, TRefFrom>> Add(Reference<TRefTo, TRefFrom> reference)
        {
            return ReferenceList.AddFirst(reference);
        }

        public void Clear()
        {
            var node = ReferenceList.First;

            while (node != null)
            {
                var next = node.Next;
                node.Value.Invalidate();
                node = next;
            }
        }

        public bool Contains(Reference<TRefTo, TRefFrom> reference)
        {
            return ReferenceList.Contains(reference);
        }
    }
}