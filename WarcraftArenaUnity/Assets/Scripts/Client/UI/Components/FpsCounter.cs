using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace Client
{
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private TextMeshProUGUI fpsLabel;
        [SerializeField, UsedImplicitly] private float alpha = 0.5f;
        [SerializeField, UsedImplicitly] private float updateInterval = 0.33f;

        private readonly char[] fpsCharArray = new char[0];
        private float timeScaledFrames;
        private int intervalFrames;
        private float intervalTimeLeft;

        [UsedImplicitly]
        private void Start()
        {
            intervalTimeLeft = updateInterval;
        }

        [UsedImplicitly]
        private void Update()
        {
            intervalTimeLeft -= Time.deltaTime;
            timeScaledFrames += Time.timeScale / Time.deltaTime;
            ++intervalFrames;

            if (intervalTimeLeft <= 0.0)
            {
                int fps = Mathf.FloorToInt(timeScaledFrames / intervalFrames);
                fpsLabel.SetCharArray(fpsCharArray.SetIntNonAlloc(fps, out int length), 0, length);

                if (fps < 30)
                    fpsLabel.color = new Color(Color.red.r, Color.red.g, Color.red.b, alpha);
                else if (fps < 55)
                    fpsLabel.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, alpha);
                else
                    fpsLabel.color = new Color(Color.green.r, Color.green.g, Color.green.b, alpha);

                intervalTimeLeft = updateInterval;
                timeScaledFrames = 0.0F;
                intervalFrames = 0;
            }
        }
    }
}