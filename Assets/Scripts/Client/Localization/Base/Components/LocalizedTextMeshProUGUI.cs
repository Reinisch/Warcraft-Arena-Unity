using JetBrains.Annotations;
using UnityEngine;
using Common;
using TMPro;

namespace Client.Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedTextMeshProUGUI : LocalizedBehaviour
    {
        [SerializeField, UsedImplicitly, HideInInspector] private TextMeshProUGUI textMeshPro;

        public TextMeshProUGUI TextMeshPro => textMeshPro;

        [UsedImplicitly]
        private void OnValidate()
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();

            Assert.IsNotNull(textMeshPro, $"Broken localization component reference at {this.GetPath()}");
        }

        internal override void Localize()
        {
            textMeshPro.text = LocalizedValue;
        }
    }
}
