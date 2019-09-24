using System.Collections.Generic;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Common;

namespace Client
{
    [CreateAssetMenu(fileName = "Spell Overlay Reference", menuName = "Game Data/Scriptable/Spell Overlay", order = 10)]
    public class SpellOverlayReference : ScriptableReferenceClient, IScreenHandler<BattleScreen>, IVisibleAuraHandler
    {
        [SerializeField, UsedImplicitly] private InterfaceReference reference;
        [SerializeField, UsedImplicitly] private SpellOverlaySettingsContainer spellOverlaySettingsContainer;

        private readonly Dictionary<int, SpellOverlaySettings> overlaySettingsByAuraId = new Dictionary<int, SpellOverlaySettings>();
        private readonly Dictionary<int, List<IVisibleAura>> activeAurasById = new Dictionary<int, List<IVisibleAura>>();
        private readonly Dictionary<int, SpellOverlay> activeSpellOverlaysByAuraId = new Dictionary<int, SpellOverlay>();

        private BattleScreen battleScreen;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            for (int i = 0; i < spellOverlaySettingsContainer.ItemList.Count; i++)
                overlaySettingsByAuraId.Add(spellOverlaySettingsContainer.ItemList[i].TriggerAura.Id, spellOverlaySettingsContainer.ItemList[i]);

            reference.AddHandler(this);
        }

        protected override void OnUnregister()
        {
            reference.RemoveHandler(this);

            overlaySettingsByAuraId.Clear();

            Assert.IsTrue(activeAurasById.Count == 0);
            Assert.IsTrue(activeSpellOverlaysByAuraId.Count == 0);

            base.OnUnregister();
        }

        protected override void OnPlayerControlGained(Player player)
        {
            base.OnPlayerControlGained(player);

            player.FindBehaviour<AuraControllerClient>().AddHandler(this);
        }

        protected override void OnPlayerControlLost(Player player)
        {
            player.FindBehaviour<AuraControllerClient>().RemoveHandler(this);

            base.OnPlayerControlLost(player);
        }

        private void HandleSpellOverlay(int auraId, bool addedAura)
        {
            if (battleScreen == null)
                return;

            if (addedAura)
            {
                if (!activeSpellOverlaysByAuraId.ContainsKey(auraId))
                {
                    activeSpellOverlaysByAuraId[auraId] = GameObjectPool.Take(overlaySettingsByAuraId[auraId].Prototype);
                    activeSpellOverlaysByAuraId[auraId].RectTransform.SetParentAndReset(battleScreen.FindTag(BattleHudTagType.SpellOverlay));
                    activeSpellOverlaysByAuraId[auraId].ModifyState(SpellOverlay.State.Active);
                }
            }
            else
            {
                if (activeSpellOverlaysByAuraId.TryGetValue(auraId, out SpellOverlay spellOverlay))
                {
                    activeSpellOverlaysByAuraId[auraId].ModifyState(SpellOverlay.State.Disabled);
                    activeSpellOverlaysByAuraId.Remove(auraId);
                    GameObjectPool.Return(spellOverlay, false);
                }
            }
        }

        void IScreenHandler<BattleScreen>.OnScreenShown(BattleScreen screen)
        {
            battleScreen = screen;

            foreach (var auraEntry in activeAurasById)
                HandleSpellOverlay(auraEntry.Key, true);
        }

        void IScreenHandler<BattleScreen>.OnScreenHide(BattleScreen screen)
        {
            foreach (var auraEntry in activeAurasById)
                HandleSpellOverlay(auraEntry.Key, false);

            battleScreen = null;
        }

        void IVisibleAuraHandler.AuraApplied(IVisibleAura visibleAura)
        {
            if (overlaySettingsByAuraId.ContainsKey(visibleAura.AuraId))
            {
                activeAurasById.Insert(visibleAura.AuraId, visibleAura);
                if (battleScreen != null)
                    HandleSpellOverlay(visibleAura.AuraId, true);
            }
        }

        void IVisibleAuraHandler.AuraUnapplied(IVisibleAura visibleAura)
        {
            if (overlaySettingsByAuraId.ContainsKey(visibleAura.AuraId))
            {
                activeAurasById.Delete(visibleAura.AuraId, visibleAura);
                HandleSpellOverlay(visibleAura.AuraId, false);
            }
        }

        void IVisibleAuraHandler.AuraRefreshed(IVisibleAura visibleAura)
        {
        }
    }
}