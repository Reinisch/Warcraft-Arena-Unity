using Common;
using Core;
using UnityEngine;

namespace Client
{
    public partial class AuraControllerClient
    {
        private class Slot : IVisibleAura
        {
            private readonly AuraControllerClient controller;

            public int AuraId { get; private set; }
            public int DurationMax { get; private set; }
            public int DurationLeft { get; private set; }
            public int Charges { get; private set; }

            public bool HasActiveAura => AuraId > 0;

            public Slot(AuraControllerClient controller)
            {
                this.controller = controller;
            }

            public void DoUpdate(int deltaTime)
            {
                if (HasActiveAura && DurationLeft > 0)
                {
                    if (DurationLeft >= deltaTime)
                        DurationLeft -= deltaTime;
                    else
                        DurationLeft = 0;
                }
            }

            public void SetState(NetAuraSlot aura)
            {
                if (!aura.HasAura)
                {
                    if (HasActiveAura)
                    {
                        HandleUnapplication();
                        AuraId = 0;
                    }

                    return;
                }

                int oldAuraId = AuraId;
                int oldCharges = Charges;

                if (oldAuraId != aura.AuraId)
                    HandleUnapplication();

                AuraId = aura.AuraId;
                DurationMax = aura.DurationMax;
                DurationLeft = aura.DurationLeft;
                Charges = aura.Charges;

                if (oldAuraId == AuraId && oldCharges != Charges)
                    HandleRefresh();
                if (oldAuraId != AuraId)
                    HandleApplication();
            }

            private void HandleApplication()
            {
                controller.HandleApplication(this);
            }

            private void HandleUnapplication()
            {
                controller.HandleUnapplication(this);
            }

            private void HandleRefresh()
            {
                controller.HandleRefresh(this);
            }
        }
    }
}
