using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public float unitSpeedLimit = 30f;
    [SerializeField]
    private PhysicMaterial groundedUnitMaterial;
    [SerializeField]
    private PhysicMaterial slidingUnitMaterial;

    public static GameManager Instanse { get; set; }

    public static PhysicMaterial GroundedMaterial { get { return Instanse.groundedUnitMaterial; } }
    public static PhysicMaterial SlidingMaterial { get { return Instanse.slidingUnitMaterial; } }
    public static float UnitSpeedLimit { get { return Instanse.unitSpeedLimit; } }

    void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else
        {
            Destroy(this);
            return;
        }
    }
}
