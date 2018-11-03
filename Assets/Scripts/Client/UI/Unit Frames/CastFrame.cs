using Core;
using UnityEngine;

public class CastFrame : MonoBehaviour
{
    public CastBar castBar;

    void Awake()
    {
        castBar = transform.Find("Cast Bar").gameObject.GetComponent<CastBar>();
    }

    public void UpdateUnit()
    {
        /*if (gameObject.activeSelf)
        {
            if (castBar.caster == null || castBar.caster.Character.SpellCast.HasCast == false)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (castBar.caster != null && castBar.caster.Character.SpellCast.HasCast == true)
            {
                gameObject.SetActive(true);
                spellText.text = castBar.caster.Character.SpellCast.spell.name;
            }
        }*/
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
