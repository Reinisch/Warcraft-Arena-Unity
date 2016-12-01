using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitFrame : MonoBehaviour
{
    public Unit unit;

    public FillBar health;
    public FillBar mainResource;

    Text unitName;

    public void Initialize()
    {
        health = transform.FindChild("HealthBar").gameObject.GetComponent<FillBar>();
        unitName = transform.FindChild("Top Panel").FindChild("Unit Name").GetComponent<Text>();
        health.Initialize();
        mainResource = transform.FindChild("ResourceBar").gameObject.GetComponent<FillBar>();
        mainResource.Initialize();
    }

    public void UpdateFrame()
    {
        health.UpdateBar();
        mainResource.UpdateBar();
    }
    public void SetUnit(Unit newUnit)
    {
        unit = newUnit;
        if (unit != null)
        {
            gameObject.SetActive(true);
            health.SetAttribute(newUnit.Character.health);
            mainResource.SetAttribute(newUnit.Character.mainResourse);
            unitName.text = newUnit.unitName + "-" + newUnit.character.className;
        }
        else
        {
            gameObject.SetActive(false);
            health.SetAttribute(null);
            mainResource.SetAttribute(null);
            unitName.text = "";
        }
    }

    public void OnTargetSet(Unit target)
    {
        gameObject.SetActive(true);
        unit = target;
        health.SetAttribute(target.Character.health);
        mainResource.SetAttribute(target.Character.mainResourse);
        unitName.text = target.unitName + "-" + target.character.className;
    }
    public void OnTargetLost(Unit target)
    {
        gameObject.SetActive(false);
        unit = null;
        health.SetAttribute(null);
        mainResource.SetAttribute(null);
        unitName.text = "";
    }
    public void OnTargetSwitch(Unit target)
    {
        unit = target;

        health.SetAttribute(target.Character.health);
        mainResource.SetAttribute(target.Character.mainResourse);
        unitName.text = target.unitName + "-" + target.character.className;
    }

}
