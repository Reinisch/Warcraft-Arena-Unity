using Common;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Arena.Editor
{
    [UsedImplicitly]
    internal class ScriptableAssetPostProcessor : AssetPostprocessor
    {
        internal static bool HasDeletedPostprocessableAssets { get; set; }

        [UsedImplicitly]
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            bool hasChanged = false;
            foreach (string path in importedAssets)
                foreach(var asset in AssetDatabase.LoadAllAssetsAtPath(path))
                    if (asset is IScriptablePostProcess postProcessable)
                        hasChanged |= postProcessable.OnPostProcess(false);

            if (HasDeletedPostprocessableAssets)
            {
                HasDeletedPostprocessableAssets = false;
                hasChanged = true;
            }

            if (hasChanged)
                AssetDatabase.SaveAssets();
        }
    }

    [UsedImplicitly]
    internal class ScriptableAssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        [UsedImplicitly]
        private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions removeOptions)
        {
            var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (asset is IScriptablePostProcess postProcessable)
            {
                ScriptableAssetPostProcessor.HasDeletedPostprocessableAssets = true;
                postProcessable.OnPostProcess(true);
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}
