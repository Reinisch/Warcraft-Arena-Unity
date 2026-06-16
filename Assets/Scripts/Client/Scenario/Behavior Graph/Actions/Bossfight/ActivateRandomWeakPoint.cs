using System;
using System.Collections.Generic;
using Common;
using Core;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Zenject;

namespace Client.BehaviorGraph
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Activate Random Weak Point",
        description: "Activates a random set of inactive weak points on a unit's model.",
        story: "Activate [WeakPointAmount] weak points on [Unit]",
        category: "Action/Unit",
        id: "d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8a9")]
    public class ActivateRandomWeakPoint : BehaviourGraphAction
    {
        [SerializeReference] public BlackboardVariable<Unit> Unit;
        [SerializeReference] public BlackboardVariable<int> WeakPointDamageLimit;
        [SerializeReference] public BlackboardVariable<float> WeakPointDamageMulti = new(20f);
        [SerializeReference] public BlackboardVariable<int> WeakPointAmount;

        [Inject]
        private RenderingReference rendering;

        private readonly List<UnitWeakPointHitBox> candidates = new();

        protected override Status OnStart()
        {
            if (!rendering.UnitRenderers.TryFind(Unit.Value, out UnitRenderer unitRenderer) || unitRenderer.Model == null)
                return Status.Success;

            candidates.Clear();
            foreach (UnitWeakPointHitBox weakPoint in unitRenderer.Model.WeakPoints)
                if (!weakPoint.isActiveAndEnabled)
                    candidates.Add(weakPoint);

            if (candidates.Count == 0)
                return Status.Success;

            int amountToActivate = WeakPointAmount.Value;
            while (candidates.Count > 0 && amountToActivate > 0)
            {
                UnitWeakPointHitBox candidate = RandomUtils.GetRandomElement(candidates);
                candidate.Activate(WeakPointDamageLimit.Value, WeakPointDamageMulti.Value);
                candidates.Remove(candidate);
                amountToActivate--;
            }

            return Status.Success;
        }
    }
}
