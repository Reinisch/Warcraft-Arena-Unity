using Bolt;
using UnityEngine;
using UnityEngine.UI;
using Core;
using JetBrains.Annotations;

namespace Client
{
    public class BuffDisplayFrame : MonoBehaviour 
    {
        [SerializeField, UsedImplicitly] private BuffSlot buffSlotPrototype;
        [SerializeField, UsedImplicitly] private GridLayoutGroup grid;
        [SerializeField, UsedImplicitly] private CanvasGroup canvasGroup;
        [SerializeField, UsedImplicitly] private int buffRows;
        [SerializeField, UsedImplicitly] private int buffColls;

        private BuffSlot[] buffSlots;
        private Unit unit;
        private int maxBuffs;

        [UsedImplicitly]
        private void Awake()
        {
            buffSlots = new BuffSlot[buffRows * buffColls];
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = buffColls;

            for (int i = 0; i < buffRows * buffColls; i++)
            {
                buffSlots[i] = Instantiate(buffSlotPrototype, transform);
                buffSlots[i].UpdateState(null);
            }

            float cellSize = transform.GetComponent<RectTransform>().rect.width / buffColls;
            grid.cellSize = new Vector2(cellSize, cellSize);
        }

        public void DoUpdate(float deltaTime)
        {
            for (int i = 0; i < buffSlots.Length; i++)
                buffSlots[i].DoUpdate(deltaTime);
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

            int visibleBuffs = Mathf.Min(buffSlots.Length, unit.EntityState.VisibleAuras.Length);
            for (int i = 0; i < visibleBuffs; i++)
                buffSlots[i].UpdateState(unit.EntityState.VisibleAuras[i]);

            for (int i = visibleBuffs; i < buffSlots.Length; i++)
                buffSlots[i].UpdateState(null);

            unit.EntityState.AddCallback($"{nameof(unit.EntityState.VisibleAuras)}[]", OnVisibleAurasChanged);
        }

        private void DeinitializeUnit()
        {
            unit.EntityState.RemoveCallback($"{nameof(unit.EntityState.VisibleAuras)}[]", OnVisibleAurasChanged);

            for (int i = 0; i < buffSlots.Length; i++)
                buffSlots[i].UpdateState(null);

            unit = null;
        }

        private void OnVisibleAurasChanged(IState state, string propertyPath, ArrayIndices arrayIndices)
        {
            for (int i = 0; i < arrayIndices.Length; i++)
            {
                int index = arrayIndices[i];

                if (index < buffSlots.Length)
                    buffSlots[index].UpdateState(unit.EntityState.VisibleAuras[index]);
            }
        }
    }
}