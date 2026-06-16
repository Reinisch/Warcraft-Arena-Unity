using UnityEngine;

namespace Client
{
    public class CameraReference : ScriptableReferenceClient
    {
        [field:SerializeField]
        public WarcraftCamera WarcraftCamera { get; private set; }

        [field:SerializeField]
        public MinimapCamera MinimapCamera { get; private set; }

        public override void OnControlStateChanged(bool underControl)
        {
            if (underControl)
            {
                base.OnControlStateChanged(true);

                Player.EventTeleported += OnTeleported;
                WarcraftCamera.Target = Player;
                MinimapCamera.Target = Player;
            }
            else
            {
                Player.EventTeleported -= OnTeleported;
                WarcraftCamera.Target = null;
                MinimapCamera.Target = null;

                base.OnControlStateChanged(false);
            }

            void OnTeleported()
            {
                WarcraftCamera.UpdateTargetPosition(true);
            }
        }
    }
}
