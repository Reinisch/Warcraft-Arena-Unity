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
            transformByInterfaceCanvasType.Populate();
        }

        public void Unregister()
        {
            transformByInterfaceCanvasType.Clear();
        }

        public RectTransform FindRoot(InterfaceCanvasType canvasType) => transformByInterfaceCanvasType.Value(canvasType);
    }
}
