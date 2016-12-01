using System;
using UnityEngine;

public class FadeAura : IAura
{
    public float fadeCoeff;
    private Material unitMaterial;

    public FadeAura(float newFadeCoeff)
    {
        fadeCoeff = newFadeCoeff;
    }

    public void Apply(Unit unit)
    {
        unitMaterial = unit.gameObject.GetComponent<SpriteRenderer>().material;
        unitMaterial.color = new Color(unitMaterial.color.r, unitMaterial.color.g, unitMaterial.color.b, unitMaterial.color.a * fadeCoeff);
    }
    public void Reverse(Unit unit)
    {
        unitMaterial.color = new Color(unitMaterial.color.r, unitMaterial.color.g, unitMaterial.color.b, unitMaterial.color.a / fadeCoeff);
    }


    public IAura Clone(Buff newBuff)
    {
        return new FadeAura(fadeCoeff);
    }
    public void Dispose()
    {
        unitMaterial = null;
    }
}