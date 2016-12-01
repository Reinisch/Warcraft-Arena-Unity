using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class BuffBar : MonoBehaviour 
{
    public GameObject buffIcon;

    public int buffRows;
    public int buffColls;
    public int buffCount;

    GridLayoutGroup grid;
    BuffIcon[] buffIcons;
    ArenaManager world;

    public Unit BuffTrackTarget { get; private set; }

    public void Initialize(Unit target)
    {
        BuffTrackTarget = target;
        buffIcons = new BuffIcon[buffRows * buffColls];
        world = ArenaManager.Instanse;
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
        int currentBuffCount = BuffTrackTarget.character.Buffs.Count;
        if (currentBuffCount > buffCount)
            currentBuffCount = buffCount;

        for (int i = 0; i < currentBuffCount; i++)
        {
            buffIcons[i].gameObject.SetActive(true);
            buffIcons[i].IconImage.sprite = world.SpellIcons[BuffTrackTarget.character.Buffs[i].iconName];
            double timer = BuffTrackTarget.character.Buffs[i].timeLeft;
            if (timer > 1)
                timer = Math.Round(timer);
            else
                timer = Math.Round(timer, 1);
            buffIcons[i].TimerText.text = timer.ToString();
        }
        for (int i = currentBuffCount; i < buffCount; i++)
        {
            buffIcons[i].gameObject.SetActive(false);
            buffIcons[i].TimerText.text = "";
        }
    }

    public void OnScreenResize()
    {
        ResizeIcons();
    }

    void ResizeIcons()
    {
        float cellSize = transform.GetComponent<RectTransform>().rect.width / (float)buffColls;
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
