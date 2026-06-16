using System.Collections.Generic;
using Client.UI;
using Common;
using Core;
using Zenject;

namespace Client
{
    public class ActionErrorDisplayPresenter : Presenter<ActionErrorDisplay>
    {
        [Inject] private EventBus eventBus;

        private readonly List<ActionErrorItem> activeErrors = new();

        public void Activate()
        {
            View.PreinstantiateItems();

            eventBus.RegisterEvent<SpellCastResult>(GameEvents.ClientSpellFailed, OnClientSpellFailed);
        }

        public void Deactivate()
        {
            eventBus.UnregisterEvent<SpellCastResult>(GameEvents.ClientSpellFailed, OnClientSpellFailed);

            for (int i = activeErrors.Count - 1; i >= 0; i--)
                View.ReturnError(activeErrors[i], true);

            activeErrors.Clear();
        }

        public override void Tick(float deltaTime)
        {
            for (int i = activeErrors.Count - 1; i >= 0; i--)
            {
                if (activeErrors[i].DoUpdate(deltaTime))
                {
                    View.ReturnError(activeErrors[i], false);
                    activeErrors.RemoveAt(i);
                }
            }
        }

        private void OnClientSpellFailed(SpellCastResult castResult)
        {
            if (!View.AllowRepeating)
                foreach (ActionErrorItem item in activeErrors)
                    if (item.CastResult == castResult)
                        return;

            View.PlayAppearSound();

            activeErrors.Add(View.SpawnError(castResult));
        }
    }
}
