using System.Collections.Generic;

namespace Core
{
    public abstract partial class Unit
    {
        internal class AuraVisibleController : IUnitBehaviour
        {
            private IUnitState unitState;

            private readonly Dictionary<AuraApplication, int> visibleSlotsByApplication = new Dictionary<AuraApplication, int>();
            private readonly List<AuraApplication> unslottedApplications = new List<AuraApplication>();
            private readonly List<int> availableSlots = new List<int>();
            private AuraApplication[] applicationSlots;

            internal bool NeedUpdate { private get; set; }

            public bool HasClientLogic => false;
            public bool HasServerLogic => true;

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
                    if (visibleSlotsByApplication.TryGetValue(auraApplication, out int occupiedSlotIndex))
                    {
                        visibleSlotsByApplication.Remove(auraApplication);

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
                if (!NeedUpdate)
                    return;

                NeedUpdate = false;

                for (int i = 0; i < applicationSlots.Length; i++)
                {
                    AuraApplication applicationInSlot = applicationSlots[i];
                    if (applicationInSlot == null)
                        unitState.VisibleAuras[i].AuraId = 0;
                    else
                    {
                        unitState.VisibleAuras[i].AuraId = applicationInSlot.Aura.AuraInfo.Id;
                        unitState.VisibleAuras[i].RefreshFrame = applicationInSlot.Aura.RefreshServerFrame;
                        unitState.VisibleAuras[i].Duration = applicationInSlot.Aura.RefreshDuration;
                        unitState.VisibleAuras[i].MaxDuration = applicationInSlot.Aura.MaxDuration;
                        unitState.VisibleAuras[i].Charges = applicationInSlot.Aura.Charges;
                    }
                }
            }

            void IUnitBehaviour.HandleUnitAttach(Unit unit)
            {
                unitState = unit.entityState;

                applicationSlots = new AuraApplication[unitState.VisibleAuras.Length];
                for (int i = 0; i < unitState.VisibleAuras.Length; i++)
                    availableSlots.Add(i);
            }

            void IUnitBehaviour.HandleUnitDetach()
            {
                availableSlots.Clear();
                unslottedApplications.Clear();
                visibleSlotsByApplication.Clear();

                applicationSlots = null;
                unitState = null;
            }
        }
    }
}
