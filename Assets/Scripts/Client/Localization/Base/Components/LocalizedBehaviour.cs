using JetBrains.Annotations;
using UnityEngine;

namespace Client.Localization
{
    internal abstract class LocalizedBehaviour : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] protected LocalizedString LocalizedString;

        [UsedImplicitly]
        private void Awake()
        {
            LocalizationReference.AddBehaviour(this);
        }

        [UsedImplicitly]
        private void OnDestroy()
        {
            LocalizationReference.RemoveBehaviour(this);
        }

        internal abstract void Localize();
    }
}
