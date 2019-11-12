using JetBrains.Annotations;
using UnityEngine;
using Common;
using TMPro;

namespace Client.Localization
{
    [RequireComponent(typeof(TextMeshPro))]
    public class LocalizedTextMeshPro : LocalizedBehaviour
    {
        [SerializeField, UsedImplicitly, HideInInspector] private TextMeshPro textMeshPro;

        [UsedImplicitly]
        private void OnValidate()
        {
            textMeshPro = GetComponent<TextMeshPro>();

            Assert.IsNotNull(textMeshPro, $"Broken localization component reference at {this.GetPath()}");
        }

        internal override void Localize()
        {
            textMeshPro.text = LocalizedValue;
        }
    }
}
