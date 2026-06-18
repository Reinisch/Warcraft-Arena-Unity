using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace Client.UI
{
    public class ArenaResultRow : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private TMP_Text nameLabel;
        [SerializeField, UsedImplicitly] private TMP_Text damageLabel;
        [SerializeField, UsedImplicitly] private TMP_Text healingLabel;

        public void Set(ArenaParticipantInfo info, Color teamColor)
        {
            if (nameLabel != null)
            {
                nameLabel.text = info.IsLocalPlayer ? $"{info.Name} (You)" : info.Name;
                nameLabel.color = teamColor;
            }

            if (damageLabel != null)
                damageLabel.text = info.DamageDone.ToString("N0");

            if (healingLabel != null)
                healingLabel.text = info.HealingDone.ToString("N0");
        }
    }
}
