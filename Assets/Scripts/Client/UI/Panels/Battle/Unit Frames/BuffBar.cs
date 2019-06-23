using UnityEngine;
using UnityEngine.UI;
using Core;
using JetBrains.Annotations;

namespace Client
{
    public class BuffBar : MonoBehaviour 
    {
        [SerializeField, UsedImplicitly] private GameObject buffSlotPrototype;
        [SerializeField, UsedImplicitly] private GridLayoutGroup grid;
        [SerializeField, UsedImplicitly] private int buffRows;
        [SerializeField, UsedImplicitly] private int buffColls;
        [SerializeField, UsedImplicitly] private int buffCount;

        private GameObject[] buffSlots;

        public void Initialize(Unit target)
        {
            buffSlots = new GameObject[buffRows * buffColls];
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = buffColls;

            for (int i = 0; i < buffRows * buffColls; i++)
            {
                buffSlots[i] = Instantiate(buffSlotPrototype, transform);
                buffSlots[i].gameObject.SetActive(false);
            }

            ResizeIcons();
        }

        public void OnScreenResize()
        {
            ResizeIcons();
        }

        private void ResizeIcons()
        {
            float cellSize = transform.GetComponent<RectTransform>().rect.width / buffColls;
            grid.cellSize = new Vector2(cellSize, cellSize);
        }
    }
}