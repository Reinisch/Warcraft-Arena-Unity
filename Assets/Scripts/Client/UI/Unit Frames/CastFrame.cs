using Core;
using JetBrains.Annotations;
using UnityEngine;

public class CastFrame : MonoBehaviour
{
    [SerializeField, UsedImplicitly] private CastBar castBar;

    [UsedImplicitly]
    private void Awake()
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

        castBar.SetCaster(target);
    }

    public void OnTargetLost(Unit target)
    {
        gameObject.SetActive(false);

        castBar.SetCaster(null);
    }

    public void OnTargetSwitch(Unit target)
    {
        castBar.SetCaster(target);
    }
}
