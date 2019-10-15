using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

[UsedImplicitly]
public class ScrollRectVelocityModifier : MonoBehaviour
{
    [SerializeField, UsedImplicitly] private ScrollRect targetScrollRect;
    [SerializeField, UsedImplicitly] private Vector2 velocity;
    [SerializeField, UsedImplicitly] private bool repeatWithDelay;
    [SerializeField, UsedImplicitly] private float repeatVelocityDelay = 0.1f;

    [UsedImplicitly]
    public void Forward()
    {
        ModifyVelocityForward();

        if (repeatWithDelay)
        {
            CancelInvoke();
            Invoke(nameof(ModifyVelocityForward), repeatVelocityDelay);
        }
    }

    [UsedImplicitly]
    public void Backward()
    {
        ModifyVelocityBackward();

        if (repeatWithDelay)
        {
            CancelInvoke();
            Invoke(nameof(ModifyVelocityBackward), repeatVelocityDelay);
        }
    }

    private void ModifyVelocityForward() => targetScrollRect.velocity = velocity;

    private void ModifyVelocityBackward() => targetScrollRect.velocity = -velocity;
}