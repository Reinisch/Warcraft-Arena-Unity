using UnityEditor;
using UnityEngine;

public class BoltPostProcessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            if (str.Contains("project.xml") || str.Contains("project.bytes"))
            {
               // Debug.Log("Bolt project file changed outside of Unity, reloading it.");
               // BoltWindow.ProjectNeedsReloading = true;
            }
        }
    }
}