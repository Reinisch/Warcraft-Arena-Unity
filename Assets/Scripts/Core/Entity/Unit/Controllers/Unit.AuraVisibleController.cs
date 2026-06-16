using System;
using System.Collections.Generic;

namespace Core
{
    public abstract partial class Unit
    {
        public const int MaxVisibleAuraSlots = 30;

        public class AuraVisibleController : IUnitBehaviour, ILogicBehaviour
        {
            private readonly Dictionary<AuraApplication, int> visibleSlotsByApplication = new();
            private readonly List<AuraApplication> unslottedApplications = new();
            private readonly List<int> availableSlots = new();
            private AuraApplication[] applicationSlots;

            internal bool NeedUpdate { private get; set; }

            public AuraApplication[] ApplicationSlots => applicationSlots;
            // Client too: it is the visible-aura slot model the client's AuraControllerClient renders from.
            // Without it the client behaviour reads null slots (NPE); on a pure client it stays empty until
            // aura state is replicated.
            public bool HasClientLogic => true;
            public bool HasServerLogic => true;

            public event Action EventVisibleAurasChanged;

            internal void HandleAuraApplication(AuraApplication auraApplication, bool applied)
            {
                if (auraApplication.Aura.SpellInfo.IsPassive)
                    return;

                if (applied)
                {
                    if (availableSlots.Count == 0)
                        unslottedApplications.Add(auraApplication);
                    else
                    {
                        visibleSlotsByApplication[auraApplication] = availableSlots[0];
                        applicationSlots[availableSlots[0]] = auraApplication;
                        availableSlots.RemoveAt(0);
                    }
                }
                else
                {
                    if (visibleSlotsByApplication.Remove(auraApplication, out int occupiedSlotIndex))
                    {
                        if (unslottedApplications.Count == 0)
                        {
                            availableSlots.Add(occupiedSlotIndex);
                            applicationSlots[occupiedSlotIndex] = null;
                        }
                        else
                        {
                            applicationSlots[occupiedSlotIndex] = unslottedApplications[0];
                            visibleSlotsByApplication[unslottedApplications[0]] = occupiedSlotIndex;
                            unslottedApplications.RemoveAt(0);
                        }
                    }
                    else
                        unslottedApplications.Remove(auraApplication);
                }

                NeedUpdate = true;
            }

            void IUnitBehaviour.DoUpdate(int deltaTime)
            {
                if (NeedUpdate)
                {
                    NeedUpdate = false;
                    EventVisibleAurasChanged?.Invoke();
                }
            }

            /// <summary>Client-side: replicated auras changed — re-raise so the client aura visuals refresh.</summary>
            internal void NotifyChanged() => EventVisibleAurasChanged?.Invoke();

            void IUnitBehaviour.HandleUnitAttach(Unit unit)
            {
                applicationSlots = new AuraApplication[MaxVisibleAuraSlots];
                for (int i = 0; i < MaxVisibleAuraSlots; i++)
                    availableSlots.Add(i);
            }

            void IUnitBehaviour.HandleUnitDetach()
            {
                availableSlots.Clear();
                unslottedApplications.Clear();
                visibleSlotsByApplication.Clear();

                applicationSlots = null;
            }
        }
    }
}
