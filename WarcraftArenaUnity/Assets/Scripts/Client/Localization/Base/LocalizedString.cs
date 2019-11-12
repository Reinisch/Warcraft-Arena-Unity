using Common;
using UnityEngine;

namespace Client.Localization
{
    [CreateAssetMenu(fileName = "Localized String", menuName = "Game Data/Localization/Localized String", order = 1)]
    public class LocalizedString : ScriptableObject
    {
        private string localizedValue;

        public string Value
        {
            get
            {
                Assert.IsNotNull(localizedValue, $"Using non localized string: {name}");

                return localizedValue ?? name;
            }
            internal set => localizedValue = value;
        }
    }
}
