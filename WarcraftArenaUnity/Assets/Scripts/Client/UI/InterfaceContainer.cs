using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class InterfaceContainer : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private RectTransform root;
        [SerializeField, UsedImplicitly] private TransformCanvasTypeDictionary transformByInterfaceCanvasType;

        public RectTransform Root => root;

        public void Register()
        {
            transformByInterfaceCanvasType.Register();
        }

        public void Unregister()
        {
            transformByInterfaceCanvasType.Unregister();
        }

        public RectTransform FindRoot(InterfaceCanvasType canvasType) => transformByInterfaceCanvasType.Value(canvasType);
    }
}
