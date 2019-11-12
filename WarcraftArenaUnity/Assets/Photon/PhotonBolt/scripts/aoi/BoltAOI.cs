using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BoltAOI : Bolt.EntityBehaviour
{
    [SerializeField]
    public float detectRadius = 32f;

    [SerializeField]
    public float releaseRadius = 64f;

    [SerializeField]
    public int updateRate = 30;

    void Update()
    {
        Graphics.DrawMesh(BoltPOI.Mesh, Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(detectRadius, detectRadius, detectRadius)), BoltPOI.MaterialAOIDetect, 0);
        Graphics.DrawMesh(BoltPOI.Mesh, Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(releaseRadius, releaseRadius, releaseRadius)), BoltPOI.MaterialAOIRelease, 0);
    }

    public override void SimulateOwner()
    {
        if (((BoltNetwork.Frame % 30) == 0) && (BoltNetwork.ScopeMode == Bolt.ScopeMode.Manual) && enabled)
        {
            if (entity.Controller != null)
            {
                BoltPOI.UpdateScope(this, entity.Controller);
            }
        }
    }
}
