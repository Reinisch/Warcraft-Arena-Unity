using System;
using Condition = Unity.Behavior.Condition;

namespace Core
{
    /// <summary>
    /// Base class for Unity Behavior condition nodes that are part of unit AI graphs.
    /// Injection is handled by ZenjectRootNodeAction traversing the graph on start.
    /// </summary>
    [Serializable]
    public abstract class BehaviourGraphCondition : Condition
    {
    }
}