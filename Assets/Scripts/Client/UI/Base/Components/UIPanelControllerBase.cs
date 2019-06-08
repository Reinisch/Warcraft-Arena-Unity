using UnityEngine;

namespace Client.UI
{
    public abstract class UIPanelControllerBase : MonoBehaviour
    {
        internal abstract void DoUpdate(int deltaTime);

        public abstract void HideAllPanels();
    }
}
