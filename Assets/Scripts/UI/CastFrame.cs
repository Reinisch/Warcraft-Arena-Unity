using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class CastFrame : MonoBehaviour
{
    public CastBar castBar;

    Text spellText;

    void Awake()
    {
        castBar = transform.FindChild("Cast Bar").gameObject.GetComponent<CastBar>();
        spellText = transform.FindChild("Spell Name").gameObject.GetComponent<Text>();
    }

    public void UpdateUnit()
    {
        if (gameObject.activeSelf)
        {
            if (castBar.caster == null || castBar.caster.character.SpellCast.HasCast == false)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (castBar.caster != null && castBar.caster.character.SpellCast.HasCast == true)
            {
                gameObject.SetActive(true);
                spellText.text = castBar.caster.character.SpellCast.spell.name;
            }
        }
        castBar.UpdateCastBar();
    }

    public void OnTargetSet(Unit target)
    {
        gameObject.SetActive(true);

        castBar.caster = target;
    }

    public void OnTargetLost(Unit target)
    {
        gameObject.SetActive(false);

        castBar.caster = null;
    }

    public void OnTargetSwitch(Unit target)
    {
        castBar.caster = target;
    }

}
