using JetBrains.Annotations;
using UnityEngine;

namespace Game.Workflow.Dedicated
{
    [CreateAssetMenu(fileName = "Dedicated Server Settings", menuName = "Server Data/Dedicated Server Settings", order = 1)]
    internal class DedicatedServerSettings : ScriptableObject
    {
        [SerializeField, UsedImplicitly, Range(1, 120)]
        private int targetFrameRate;
        [SerializeField, UsedImplicitly, Range(0.1f, 0.33f)]
        private float maximumDeltaTime;

        public void Apply()
        {
            Application.targetFrameRate = targetFrameRate;
            Time.maximumDeltaTime = maximumDeltaTime;
        }
    }
}
