using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core;
using JetBrains.Annotations;

namespace Client
{
    public class BuffDisplayFrame : MonoBehaviour, IVisibleAuraHandler
    {
        [SerializeField, UsedImplicitly] private BuffSlot buffSlotPrototype;
        [SerializeField, UsedImplicitly] private GridLayoutGroup grid;
        [SerializeField, UsedImplicitly] private CanvasGroup canvasGroup;
        [SerializeField, UsedImplicitly] private int buffRows;
        [SerializeField, UsedImplicitly] private int buffColls;

        private readonly List<IVisibleAura> visibleAuras = new List<IVisibleAura>();
        private BuffSlot[] buffSlots;

        private bool needsUpdate;
        private int maxBuffs;
        private Unit unit;

        [UsedImplicitly]
        private void Awake()
        {
            buffSlots = new BuffSlot[buffRows * buffColls];
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = buffColls;

            for (int i = 0; i < buffRows * buffColls; i++)
            {
                buffSlots[i] = Instantiate(buffSlotPrototype, transform);
                buffSlots[i].UpdateAura(null);
            }

            float cellSize = transform.GetComponent<RectTransform>().rect.width / buffColls;
            grid.cellSize = new Vector2(cellSize, cellSize);
        }

        public void DoUpdate()
        {
            if (needsUpdate)
            {
                needsUpdate = false;

                int visibleCount = Mathf.Min(buffSlots.Length, visibleAuras.Count);
                for (int i = 0; i < visibleCount; i++)
                    buffSlots[i].UpdateAura(visibleAuras[i]);

                for (int i = visibleCount; i < buffSlots.Length; i++)
                    buffSlots[i].UpdateAura(null);
            }

            for (int i = 0; i < buffSlots.Length; i++)
                buffSlots[i].DoUpdate();
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

        public void UpdateUnit(Unit newUnit)
        {
            if (unit != null)
                DeinitializeUnit();

            if (newUnit != null)
                InitializeUnit(newUnit);

            canvasGroup.blocksRaycasts = unit != null;
            canvasGroup.interactable = unit != null;
            canvasGroup.alpha = unit != null ? 1.0f : 0.0f;
        }

        private void InitializeUnit(Unit unit)
        {
            this.unit = unit;

            unit.FindBehaviour<AuraControllerClient>().AddHandler(this);
        }

        private void DeinitializeUnit()
        {
            unit.FindBehaviour<AuraControllerClient>().RemoveHandler(this);

            for (int i = 0; i < buffSlots.Length; i++)
                buffSlots[i].UpdateAura(null);

            unit = null;
        }
    }
}