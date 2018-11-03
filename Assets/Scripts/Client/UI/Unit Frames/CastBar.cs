using Core;
using UnityEngine;
using UnityEngine.UI;

public class CastBar : MonoBehaviour
{
    public Unit caster;

    void Awake()
    {
    }

    public void UpdateCastBar()
    {
        if (caster == null)
            return;

        //slider.value = (caster.Character.SpellCast.castTime - caster.Character.SpellCast.castTimeLeft) / caster.Character.SpellCast.castTime;
    }
}
