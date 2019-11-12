using JetBrains.Annotations;
using UnityEngine;

namespace Client.Localization
{
    public abstract class LocalizedBehaviour : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private LocalizedString localizedString;

        private object[] args;

        protected string LocalizedValue
        {
            get
            {
                if (localizedString == null || localizedString.Value == null)
                    return string.Empty;

                if (args != null)
                    return string.Format(localizedString.Value, args);

                return localizedString.Value;
            }
        } 

        [UsedImplicitly]
        private void Awake()
        {
            LocalizationReference.AddBehaviour(this);
        }

        [UsedImplicitly]
        private void OnDestroy()
        {
            LocalizationReference.RemoveBehaviour(this);

            args = null;
        }

        internal abstract void Localize();

        public void SetEmpty()
        {
            args = null;
            localizedString = null;

            Localize();
        }

        public void SetString(LocalizedString newString)
        {
            args = null;
            localizedString = newString;

            Localize();
        }

        public void SetString(LocalizedString newString, params object[] args)
        {
            this.args = args;
            localizedString = newString;

            Localize();
        }
    }
}
