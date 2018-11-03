using UnityEngine;
using UnityEngine.UI;
using System;
using Core;

public class BuffBar : MonoBehaviour 
{
    public GameObject buffIcon;

    public int buffRows;
    public int buffColls;
    public int buffCount;

    GridLayoutGroup grid;
    BuffIcon[] buffIcons;

    public Unit BuffTrackTarget { get; private set; }

    public void Initialize(Unit target)
    {
        BuffTrackTarget = target;
        buffIcons = new BuffIcon[buffRows * buffColls];
        grid = gameObject.GetComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = buffColls;

        for (int i = 0; i < buffRows * buffColls; i++)
        {
            BuffIcon newBuffIcon = ((GameObject)Instantiate(buffIcon)).GetComponent<BuffIcon>();
            newBuffIcon.Initialize();
            newBuffIcon.transform.SetParent(transform, false);
            buffIcons[i] = newBuffIcon;
            buffIcons[i].gameObject.SetActive(false);
        }
        ResizeIcons();
    }

    void Update()
    {
        if (BuffTrackTarget == null)
            return;
    }

    public void OnScreenResize()
    {
        ResizeIcons();
    }

    void ResizeIcons()
    {
        float cellSize = transform.GetComponent<RectTransform>().rect.width / buffColls;
        grid.cellSize = new Vector2(cellSize, cellSize);
    }

    public void OnTargetSet(Unit target)
    {
        BuffTrackTarget = target;
        gameObject.SetActive(true);
    }

    public void OnTargetLost(Unit target)
    {
        gameObject.SetActive(false);
        BuffTrackTarget = null;
    }

    public void OnTargetSwitch(Unit target)
    {
        BuffTrackTarget = target;
    }
}
