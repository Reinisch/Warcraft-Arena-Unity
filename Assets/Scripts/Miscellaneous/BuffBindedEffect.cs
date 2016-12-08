using UnityEngine;
using System.Collections;

public enum BuffBindPivot { Bottom, Center }

public class BuffBindedEffect : MonoBehaviour
{
    public Unit target;
    public BuffBindPivot pivot;
    public Buff buff;

    public void Initialize(Unit newTarget, Buff newBuff)
    {
        target = newTarget;
        buff = newBuff;

        transform.SetParent(newTarget.transform, false);

        if (pivot == BuffBindPivot.Bottom)
            transform.position = newTarget.transform.position;
        else
            transform.position = newTarget.centerPoint.position;

    }

    void Update()
    {

        if (target == null || target.IsDead() || buff.isDisposed)
        {
            target = null;
            buff = null;
            Destroy(gameObject);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward);
        }
    }
}
