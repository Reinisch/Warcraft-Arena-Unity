using System;
using System.Collections.Generic;
using Common;
using Core;

namespace Client
{
    public partial class AuraControllerClient : UnitBehaviour
    {
        private bool updateRequired;
        private Slot[] auraSlots = new Slot[Unit.MaxVisibleAuraSlots];

        private readonly List<IVisibleAura> activeAuras = new();
        private readonly List<IVisibleAuraHandler> activeAuraHandlers = new();

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

                if (updateRequired)
                {
                    auraSlots[i].SetState(Unit.GetVisibleAuraSlot(i));
                }
            }

            updateRequired = false;
        }

        protected override void OnAttach()
        {
            base.OnAttach();

            Array.Resize(ref auraSlots, Unit.MaxVisibleAuraSlots);

            for (int i = 0; i < auraSlots.Length; i++)
                auraSlots[i] = new Slot(this);

            updateRequired = true;
            Unit.VisibleAuras.EventVisibleAurasChanged += OnVisibleAurasChanged;
        }

        protected override void OnDetach()
        {
            Unit.VisibleAuras.EventVisibleAurasChanged -= OnVisibleAurasChanged;

            updateRequired = false;
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

        private void OnVisibleAurasChanged()
        {
            updateRequired = true;
        }
    }
}
