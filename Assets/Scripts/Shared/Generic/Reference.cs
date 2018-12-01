using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Core
{
    public abstract class Reference<TRefTo, TRefFrom> where TRefTo : class where TRefFrom : class
    {
        /// <summary>
        ///  Should be set only in TargetObjectBuildLink.
        /// </summary>
        protected LinkedListNode<Reference<TRefTo, TRefFrom>> Node { get; set; }

        public Reference<TRefTo, TRefFrom> Next { get { return Node != null && Node.Next != null ? Node.Next.Value : null; } }
        public Reference<TRefTo, TRefFrom> Prev { get { return Node != null && Node.Previous != null ? Node.Previous.Value : null; } }

        public TRefTo Target { get; private set; }
        public TRefFrom Source { get; private set; }

        /// <summary>
        ///  Check if target still exists.
        /// </summary>
        public bool IsValid { get { return Target != null; } }
        public bool HasNext { get { return Next != null; } }
        public bool HasPrev { get { return Prev != null; } }
        public bool IsInList { get { return Node != null; } }


        public virtual void Initialize()
        {
        
        }

        public virtual void Deinitialize()
        {
        
        }

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
            // source MUST remain !!!
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


        // Tell our target object that we have a link
        protected abstract void TargetObjectBuildLink();
        // Tell our target object, that the link is cut
        protected abstract void TargetObjectDestroyLink();
        // Tell our source object, that the link is cut (target destroyed)
        protected abstract void SourceObjectDestroyLink();
    }
}