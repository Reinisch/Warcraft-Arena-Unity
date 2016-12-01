using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class CastBar : MonoBehaviour
{
    public Unit caster;

    Slider slider;

    void Awake()
    {
        slider = transform.GetComponent<Slider>();
    }

    public void UpdateCastBar()
    {
        if (caster.character.SpellCast.HasCast)
        {
            slider.value = (caster.character.SpellCast.castTime - caster.character.SpellCast.castTimeLeft) / caster.character.SpellCast.castTime;
        }
    }
}
