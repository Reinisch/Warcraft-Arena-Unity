using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Common
{
    public abstract class Reference<TRefTo, TRefFrom> where TRefTo : class where TRefFrom : ReferenceManager<TRefTo, TRefFrom>
    {
        protected LinkedListNode<Reference<TRefTo, TRefFrom>> Node { get; set; }

        public Reference<TRefTo, TRefFrom> Next { get { return Node != null && Node.Next != null ? Node.Next.Value : null; } }
        public Reference<TRefTo, TRefFrom> Prev { get { return Node != null && Node.Previous != null ? Node.Previous.Value : null; } }

        public TRefTo Target { get; private set; }
        public TRefFrom Source { get; private set; }

        public bool IsValid { get { return Target != null; } }
        public bool HasNext { get { return Next != null; } }
        public bool HasPrev { get { return Prev != null; } }
        public bool IsInList { get { return Node != null; } }

        /// <summary>
        /// Create new link to target for source collection.
        /// </summary>
        public void Link(TRefTo target, TRefFrom source)
        {
            Assert.IsNotNull(source);

            if (IsValid)
                Unlink();

            if (target != null)
            {
                Target = target;
                Source = source;
                TargetObjectBuildLink();
            }
        }

        /// <summary>
        /// We don't need the reference anymore.
        /// Call comes from the source object.
        /// Tell our target object, that the link is cut.
        /// </summary>
        public void Unlink()
        {
            TargetObjectDestroyLink();
            Delink();

            Target = null;
            Source = null;
        }

        /// <summary>
        /// Link is invalid due to destruction of referenced target object.
        /// Call comes from the target object.
        /// Tell our source object, that the link is cut.
        /// </summary>
        public void Invalidate()
        {
            SourceObjectDestroyLink();
            Delink();

            Target = null;
        }

        /// <summary>
        /// Remove from linked list.
        /// </summary>
        private void Delink()
        {
            if (IsInList)
            {
                Node.List.Remove(Node);
                Node = null;
            }
        }

        protected void TargetObjectBuildLink()
        {
            Node = Source.Add(this);
        }

        protected void TargetObjectDestroyLink()
        {
        }

        protected void SourceObjectDestroyLink()
        {
        }
    }
}