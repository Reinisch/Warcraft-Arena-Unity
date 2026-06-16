using Client.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class InterfaceReference: ScreenController
    {
        [field: SerializeField, UsedImplicitly]
        public RectTransform NameplatesRoot { get; private set; }
    }
}