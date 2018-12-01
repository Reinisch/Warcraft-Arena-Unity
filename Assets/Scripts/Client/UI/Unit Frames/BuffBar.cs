using UnityEngine;
using UnityEngine.UI;
using Core;
using JetBrains.Annotations;

public class BuffBar : MonoBehaviour 
{
    [SerializeField, UsedImplicitly] private GameObject buffIcon;
    [SerializeField, UsedImplicitly] private int buffRows;
    [SerializeField, UsedImplicitly] private int buffColls;
    [SerializeField, UsedImplicitly] private int buffCount;

    private GridLayoutGroup grid;
    private BuffIcon[] buffIcons;

    public void Initialize(Unit target)
    {
        buffIcons = new BuffIcon[buffRows * buffColls];
        grid = gameObject.GetComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = buffColls;

        for (int i = 0; i < buffRows * buffColls; i++)
        {
            BuffIcon newBuffIcon = Instantiate(buffIcon).GetComponent<BuffIcon>();
            newBuffIcon.Initialize();
            newBuffIcon.transform.SetParent(transform, false);
            buffIcons[i] = newBuffIcon;
            buffIcons[i].gameObject.SetActive(false);
        }
        ResizeIcons();
    }

    public void OnScreenResize()
    {
        ResizeIcons();
    }

    public void OnTargetSet(Unit target)
    {
        gameObject.SetActive(true);
    }

    public void OnTargetLost(Unit target)
    {
        gameObject.SetActive(false);
    }

    public void OnTargetSwitch(Unit target)
    {
    }

    private void ResizeIcons()
    {
        float cellSize = transform.GetComponent<RectTransform>().rect.width / buffColls;
        grid.cellSize = new Vector2(cellSize, cellSize);
    }
}
