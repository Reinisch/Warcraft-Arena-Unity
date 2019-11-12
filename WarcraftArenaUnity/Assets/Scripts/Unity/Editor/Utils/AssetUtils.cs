using JetBrains.Annotations;
using UnityEditor;

namespace Editor
{
    [UsedImplicitly]
    public static class AssetUtils
    {
        [MenuItem("Assets/Reserialize All"), UsedImplicitly]
        private static void ReserializeEntireProject() => AssetDatabase.ForceReserializeAssets();
    }
}
