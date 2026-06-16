using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;

namespace Core
{
    /// <summary>
    /// Evaluates the utility score of each child once on entry, then runs them in descending
    /// utility order — succeeding when the first child succeeds, failing only when every child fails.
    /// Children that do not inherit <see cref="BehaviourGraphActionWithUtility"/> are treated as
    /// having a utility of 0.
    /// </summary>
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Utility Selector",
        description: "Runs the child with the highest utility score. Falls through to the next highest on failure. Utility is evaluated once on entry — no re-evaluation.",
        category: "Flow",
        id: "e1f2a3b4c5d6e7f8a9b0c1d2e3f4a5b6")]
    public class UtilitySelector : Composite
    {
        [CreateProperty] private List<int> sortedChildIndices = new();
        [CreateProperty] private int currentSortedIndex;

        protected override Status OnStart()
        {
            sortedChildIndices.Clear();
            for (int i = 0; i < Children.Count; i++)
                sortedChildIndices.Add(i);

            // Sort descending by utility — children without utility score last (treated as 0).
            sortedChildIndices.Sort((a, b) =>
            {
                float utilityA = Children[a] is BehaviourGraphActionWithUtility actionA ? actionA.GetUtility() : 0f;
                float utilityB = Children[b] is BehaviourGraphActionWithUtility actionB ? actionB.GetUtility() : 0f;
                return utilityB.CompareTo(utilityA);
            });

            currentSortedIndex = 0;
            return StartCurrentChild();
        }

        protected override Status OnUpdate()
        {
            Status childStatus = Children[sortedChildIndices[currentSortedIndex]].CurrentStatus;

            if (childStatus == Status.Success)
                return Status.Success;

            if (childStatus == Status.Failure)
            {
                currentSortedIndex++;
                return currentSortedIndex >= sortedChildIndices.Count
                    ? Status.Failure
                    : StartCurrentChild();
            }

            return Status.Waiting;
        }

        private Status StartCurrentChild()
        {
            if (currentSortedIndex >= sortedChildIndices.Count)
                return Status.Failure;

            Status childStatus = StartNode(Children[sortedChildIndices[currentSortedIndex]]);
            return childStatus switch
            {
                Status.Success => Status.Success,
                Status.Failure => ++currentSortedIndex >= sortedChildIndices.Count
                    ? Status.Failure
                    : StartCurrentChild(),
                Status.Running => Status.Waiting,
                _ => childStatus,
            };
        }
    }
}
