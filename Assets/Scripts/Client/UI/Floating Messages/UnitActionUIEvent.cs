using JetBrains.Annotations;
using UnityEngine;

public abstract class UnitActionUIEffect : MonoBehaviour
{
    [SerializeField, UsedImplicitly] private float floatingSpeed;
    [SerializeField, UsedImplicitly] private float duration;

    public void Initialize()
    {
    }
}