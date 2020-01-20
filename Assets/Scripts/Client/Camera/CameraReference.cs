using Core;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Camera Reference", menuName = "Game Data/Scriptable/Camera", order = 12)]
    public class CameraReference : ScriptableReferenceClient
    {
        public WarcraftCamera WarcraftCamera { get; private set; }
        public MinimapCamera MinimapCamera { get; private set; }

        protected override void OnRegistered()
        {
            base.OnRegistered();

            WarcraftCamera = FindObjectOfType<WarcraftCamera>();
            MinimapCamera = FindObjectOfType<MinimapCamera>();
        }

        protected override void OnUnregister()
        {
            WarcraftCamera = null;
            MinimapCamera = null;

            base.OnUnregister();
        }

        protected override void OnControlStateChanged(Player player, bool underControl)
        {
            if (underControl)
            {
                base.OnControlStateChanged(player, true);

                WarcraftCamera.Target = player;
                MinimapCamera.Target = player;
            }
            else
            {
                WarcraftCamera.Target = null;
                MinimapCamera.Target = null;

                base.OnControlStateChanged(player, false);
            }
        }
    }
}
