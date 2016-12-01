using System;
using UnityEngine;

public abstract class UnitActionUIEffect : MonoBehaviour
{
    protected float floatTime;

    public float floatingSpeed;
    public float duration; 
    public Unit target;

    public void Initialize()
    {
        floatTime = duration;
    }

    public void Dispose()
    {
        target = null;
    }
}