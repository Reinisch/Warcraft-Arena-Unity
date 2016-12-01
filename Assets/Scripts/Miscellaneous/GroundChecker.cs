using UnityEngine;
using System.Collections;

public class GroundChecker : MonoBehaviour
{
    public int GroundCollisions { get; set; }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            GroundCollisions++;
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            GroundCollisions--;
    }
}
