using System;
using UnityEngine;

public class SkillUseUIEvent : UnitActionUIEffect
{
    PlayerInterface playerInterface;
    SpellData spellData;

    Rect skillIconRect;
    Rect labelRect;

    Vector2 labelSize;
    Vector3 screenPoint;
    float iconSize;

    public void Initialize(SpellData newSpellData, Unit target, PlayerInterface interfaceRef)
    {
        base.Initialize();

        playerInterface = interfaceRef;
        spellData = newSpellData;
        iconSize = playerInterface.relativeSkillIconSize * Screen.width;
        labelSize = playerInterface.skillUseNameStyle.CalcSize(new GUIContent(newSpellData.spellName));

        transform.position = target.UnitCollider.bounds.max;
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);

        labelRect = new Rect(screenPoint.x - labelSize.x / 2,
            Screen.height - screenPoint.y - labelSize.y / 2, labelSize.x, labelSize.y);
        skillIconRect = new Rect(labelRect.x - iconSize,
            Screen.height - screenPoint.y - iconSize / 2, iconSize, iconSize);
    }

    public void Update()
    {
        floatTime -= Time.deltaTime;
        if (floatTime <= 0)
        {
            Dispose();
            Destroy(gameObject);
            return;
        }

        transform.Translate(0, floatingSpeed * Time.deltaTime, 0);
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);

        labelRect = new Rect(screenPoint.x - labelSize.x / 2,
            Screen.height - screenPoint.y - labelSize.y / 2, labelSize.x, labelSize.y);
        skillIconRect = new Rect(labelRect.x - iconSize,
            Screen.height - screenPoint.y - iconSize / 2, iconSize, iconSize);
    }

    public void OnGUI()
    {
        GUI.DrawTexture(skillIconRect, playerInterface.world.SpellIcons[spellData.iconName].texture);
        PlayerInterfaceHelper.DrawShadow(labelRect, spellData.spellName, playerInterface.skillUseNameStyle,
            playerInterface.skillUseNameStyle.normal.textColor, Color.black, new Vector2(1, 1));
    }

    public new void Dispose()
    {
        playerInterface = null;
        spellData = null;
        base.Dispose();
    }
}