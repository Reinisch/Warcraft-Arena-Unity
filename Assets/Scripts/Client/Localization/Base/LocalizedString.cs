using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Client.Localization
{
    [CreateAssetMenu(fileName = "Localized String", menuName = "Game Data/Localization/Localized String", order = 1)]
    public class LocalizedString : ScriptableObject
    {
        public string Value
        {
            get
            {
                if (LocalizationSettings.StringDatabase == null)
                    return name;

                return LocalizationSettings.StringDatabase.GetLocalizedString("GameStrings", name);
            }
        }

        public override string ToString() => Value;
    }
}
