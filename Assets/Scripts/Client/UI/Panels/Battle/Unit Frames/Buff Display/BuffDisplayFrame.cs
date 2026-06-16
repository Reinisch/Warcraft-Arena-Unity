using UnityEngine;
using UnityEngine.UI;
using JetBrains.Annotations;
using Common;
using Zenject;

namespace Client
{
    public class BuffDisplayFrame : MonoBehaviour
    {
        [Inject] private BuffDisplayPresenter presenter;
        [Inject] private GameObjectFactory objectFactory;

        [SerializeField, UsedImplicitly] private BuffSlot buffSlotPrototype;
        [SerializeField, UsedImplicitly] private GridLayoutGroup grid;
        [SerializeField, UsedImplicitly] private CanvasGroup canvasGroup;
        [SerializeField, UsedImplicitly] private int buffRows;
        [SerializeField, UsedImplicitly] private int buffColls;

        private BuffSlot[] buffSlots;

        public int SlotCount => buffSlots.Length;

        internal BuffDisplayPresenter Presenter => presenter;

        [UsedImplicitly]
        private void Awake()
        {
            buffSlots = new BuffSlot[buffRows * buffColls];
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = buffColls;

            for (int i = 0; i < buffRows * buffColls; i++)
            {
                buffSlots[i] = objectFactory.Create(buffSlotPrototype, transform);
                buffSlots[i].UpdateAura(null);
            }

            float cellSize = transform.GetComponent<RectTransform>().rect.width / buffColls;
            grid.cellSize = new Vector2(cellSize, cellSize);

            presenter.Initialize(this);
        }

        public void SetVisible(bool visible)
        {
            canvasGroup.blocksRaycasts = visible;
            canvasGroup.interactable = visible;
            canvasGroup.alpha = visible ? 1.0f : 0.0f;
        }

        public void SetSlotAura(int index, IVisibleAura visibleAura)
        {
            buffSlots[index].UpdateAura(visibleAura);
        }

        public void TickSlots()
        {
            for (int i = 0; i < buffSlots.Length; i++)
                buffSlots[i].DoUpdate();
        }
    }
}
