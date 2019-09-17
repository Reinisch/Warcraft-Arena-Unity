using System;
using System.Collections.Generic;
using Bolt;
using Common;
using Core;

namespace Client
{
    public partial class AuraControllerClient : UnitBehaviour
    {
        private bool[] changedSlots = new bool[30];
        private Slot[] auraSlots = new Slot[30];

        private readonly List<IVisibleAura> activeAuras = new List<IVisibleAura>();
        private readonly List<IVisibleAuraHandler> activeAuraHandlers = new List<IVisibleAuraHandler>();

        public override bool HasClientLogic => true;
        public override bool HasServerLogic => false;

        public void AddHandler(IVisibleAuraHandler visibleAuraHandler)
        {
            activeAuraHandlers.Add(visibleAuraHandler);

            foreach (IVisibleAura visibleAura in activeAuras)
                visibleAuraHandler.AuraApplied(visibleAura);
        }

        public void RemoveHandler(IVisibleAuraHandler visibleAuraHandler)
        {
            foreach (IVisibleAura visibleAura in activeAuras)
                visibleAuraHandler.AuraUnapplied(visibleAura);

            activeAuraHandlers.Remove(visibleAuraHandler);
        }

        protected override void OnUpdate(int deltaTime)
        {
            base.OnUpdate(deltaTime);

            for (int i = 0; i < auraSlots.Length; i++)
            {
                auraSlots[i].DoUpdate(deltaTime);

                if (changedSlots[i])
                {
                    auraSlots[i].SetState(Unit.VisibleAura(i));
                    changedSlots[i] = false;
                }
            }
        }

        protected override void OnAttach()
        {
            base.OnAttach();

            Array.Resize(ref changedSlots, Unit.VisibleAuraMaxCount);
            Array.Resize(ref auraSlots, Unit.VisibleAuraMaxCount);

            for (int i = 0; i < auraSlots.Length; i++)
                auraSlots[i] = new Slot(this);

            changedSlots.Fill(true);
            Unit.AddCallback($"{nameof(IUnitState.VisibleAuras)}[]", OnVisibleAurasChanged);
        }

        protected override void OnDetach()
        {
            Unit.RemoveCallback($"{nameof(IUnitState.VisibleAuras)}[]", OnVisibleAurasChanged);
            changedSlots.Fill(false);
            auraSlots.Fill(null);
            activeAuras.Clear();

            base.OnDetach();
        }

        private void HandleApplication(Slot slot)
        {
            activeAuras.Add(slot);

            foreach (IVisibleAuraHandler activeAuraHandler in activeAuraHandlers)
                activeAuraHandler.AuraApplied(slot);
        }

        private void HandleUnapplication(Slot slot)
        {
            activeAuras.Remove(slot);

            foreach (IVisibleAuraHandler activeAuraHandler in activeAuraHandlers)
                activeAuraHandler.AuraUnapplied(slot);
        }

        private void HandleRefresh(Slot slot)
        {
            foreach (IVisibleAuraHandler activeAuraHandler in activeAuraHandlers)
                activeAuraHandler.AuraRefreshed(slot);
        }

        private void OnVisibleAurasChanged(IState state, string propertyPath, ArrayIndices arrayIndices)
        {
            for (int i = 0; i < arrayIndices.Length; i++)
                changedSlots[arrayIndices[i]] = true;
        }
    }
}
