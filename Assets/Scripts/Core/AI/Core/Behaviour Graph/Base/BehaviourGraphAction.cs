using System;
using Zenject;
using Action = Unity.Behavior.Action;

namespace Core
{
    /// <summary>
    /// Base class for Unity Behavior action nodes that are part of unit AI graphs.
    /// Injection is handled by ZenjectRootNodeAction traversing the graph on start.
    /// </summary>
    [Serializable]
    public abstract class BehaviourGraphAction : Action
    {
        [Inject]
        protected World World { get; private set; }

        [Inject]
        protected BalanceReference Balance { get; private set; }
    }
}
