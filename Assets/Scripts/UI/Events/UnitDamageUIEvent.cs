using System;
using UnityEngine;

public class UnitDamageUIEvent : UnitActionUIEffect
{
    PlayerInterface playerInterface;
    Rect labelRect;

    Vector2 labelSize;
    Vector3 screenPoint;

    bool isCritical;
    int damage;
    int lastFontSize;

    public void Initialize(int skillDamage, Unit target, bool critical, PlayerInterface interfaceRef)
    {
        base.Initialize();

        playerInterface = interfaceRef;
        isCritical = critical;
        damage = skillDamage;

        transform.position = target.UnitCollider.bounds.max;
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (isCritical)
        {
            lastFontSize = playerInterface.damageLabelStyle.fontSize;
            playerInterface.damageLabelStyle.fontSize += (int)(1.5f * playerInterface.damageLabelStyle.fontSize * floatTime / duration);
            labelSize = playerInterface.damageLabelStyle.CalcSize(new GUIContent(damage.ToString()));
            playerInterface.damageLabelStyle.fontSize = lastFontSize;
        }
        else
        {
            labelSize = playerInterface.damageLabelStyle.CalcSize(new GUIContent(damage.ToString()));
        }
             
        labelRect = new Rect(screenPoint.x - labelSize.x / 2, Screen.height - screenPoint.y - labelSize.y / 2, labelSize.x, labelSize.x);     
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

        if (isCritical)
        {
            lastFontSize = playerInterface.damageLabelStyle.fontSize;
            playerInterface.damageLabelStyle.fontSize += (int)(1.5f * playerInterface.damageLabelStyle.fontSize * floatTime / duration);        
            labelSize = playerInterface.damageLabelStyle.CalcSize(new GUIContent(damage.ToString()));           
            playerInterface.damageLabelStyle.fontSize = lastFontSize;
        }
        else
        {
            labelSize = playerInterface.damageLabelStyle.CalcSize(new GUIContent(damage.ToString()));
        }

        labelRect = new Rect(screenPoint.x - labelSize.x / 2, Screen.height - screenPoint.y - labelSize.y / 2, labelSize.x, labelSize.x);
    }

    public void OnGUI()
    {
        if (isCritical)
        {
            lastFontSize = playerInterface.damageLabelStyle.fontSize;
            playerInterface.damageLabelStyle.fontSize += (int)(1.5f * playerInterface.damageLabelStyle.fontSize * floatTime / duration);
            PlayerInterfaceHelper.DrawShadow(labelRect, damage.ToString(), playerInterface.damageLabelStyle,
                playerInterface.damageLabelStyle.normal.textColor, Color.black, new Vector2(1, 1));
            playerInterface.damageLabelStyle.fontSize = lastFontSize;
        }
        else
        {
            PlayerInterfaceHelper.DrawShadow(labelRect, damage.ToString(), playerInterface.damageLabelStyle,
                playerInterface.damageLabelStyle.normal.textColor, Color.black, new Vector2(1, 1));
                
        }
    }

    public new void Dispose()
    {
        playerInterface = null;
        base.Dispose();
    }
}