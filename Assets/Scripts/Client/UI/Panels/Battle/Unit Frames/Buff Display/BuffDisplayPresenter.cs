using System.Collections.Generic;
using Client.UI;
using Core;
using UnityEngine;

namespace Client
{
    public class BuffDisplayPresenter : Presenter<BuffDisplayFrame>, IVisibleAuraHandler
    {
        private readonly List<IVisibleAura> visibleAuras = new();

        private bool needsUpdate;
        private Unit unit;
        
        public override void Tick(float deltaTime)
        {
            if (needsUpdate)
            {
                needsUpdate = false;

                int visibleCount = Mathf.Min(View.SlotCount, visibleAuras.Count);
                for (int i = 0; i < visibleCount; i++)
                    View.SetSlotAura(i, visibleAuras[i]);

                for (int i = visibleCount; i < View.SlotCount; i++)
                    View.SetSlotAura(i, null);
            }

            View.TickSlots();
        }

        public void SetUnit(Unit unitToSet)
        {
            if (unit != null)
            {
                unit.FindBehaviour<AuraControllerClient>().RemoveHandler(this);

                for (int i = 0; i < View.SlotCount; i++)
                    View.SetSlotAura(i, null);

                unit = null;
            }

            if (unitToSet != null)
            {
                unit = unitToSet;

                unit.FindBehaviour<AuraControllerClient>().AddHandler(this);
            }

            View.SetVisible(unit != null);
        }

        public void AuraApplied(IVisibleAura visibleAura)
        {
            needsUpdate = true;
            visibleAuras.Add(visibleAura);
        }

        public void AuraUnapplied(IVisibleAura visibleAura)
        {
            visibleAuras.Remove(visibleAura);
            needsUpdate = true;
        }

        public void AuraRefreshed(IVisibleAura visibleAura)
        {
            needsUpdate = true;
        }
    }
}
