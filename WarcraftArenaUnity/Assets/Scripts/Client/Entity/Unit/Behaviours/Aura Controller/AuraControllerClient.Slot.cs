using Common;
using UnityEngine;

namespace Client
{
    public partial class AuraControllerClient
    {
        private class Slot : IVisibleAura
        {
            private readonly AuraControllerClient controller;

            public int AuraId { get; private set; }
            public int ServerRefreshFrame { get; private set; }
            public int RefreshDuration { get; private set; }
            public int MaxDuration { get; private set; }
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

            public void SetState(VisibleAuraState state)
            {
                if (state == null || state.AuraId == 0)
                {
                    if (HasActiveAura)
                    {
                        HandleUnapplication();
                        AuraId = 0;
                    }

                    return;
                }

                int oldAuraId = AuraId;
                int oldRefrechFrame = ServerRefreshFrame;
                int oldCharges = Charges;

                if (oldAuraId != AuraId)
                    HandleUnapplication();

                AuraId = state.AuraId;
                ServerRefreshFrame = state.RefreshFrame;
                RefreshDuration = state.Duration;
                MaxDuration = state.MaxDuration;
                Charges = state.Charges;

                int expectedCooldownFrames = (int) (RefreshDuration / BoltNetwork.FrameDeltaTime / TimeUtils.InMilliseconds);
                int framesPassed = BoltNetwork.ServerFrame - ServerRefreshFrame;
                if (framesPassed > expectedCooldownFrames || expectedCooldownFrames < 1)
                    DurationLeft = 0;
                else
                {
                    var cooldownProgressLeft = 1.0f - (float) framesPassed / expectedCooldownFrames;
                    DurationLeft = Mathf.RoundToInt(RefreshDuration * cooldownProgressLeft);
                }

                if (oldAuraId == AuraId && (oldRefrechFrame != ServerRefreshFrame || oldCharges != Charges))
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
