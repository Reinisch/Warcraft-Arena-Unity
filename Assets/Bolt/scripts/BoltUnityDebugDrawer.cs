using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BoltInternal {
  public class UnityDebugDrawer : BoltInternal.IDebugDrawer {
    bool isEditor;

    void IDebugDrawer.IsEditor(bool isEditor) {
      this.isEditor = isEditor;
    }

    void IDebugDrawer.SelectGameObject(GameObject gameObject) {
#if UNITY_EDITOR
      if (!isEditor) {
        UnityEditor.Selection.activeGameObject = gameObject;
      }
#endif
    }

    void BoltInternal.IDebugDrawer.Indent(int level) {
#if UNITY_EDITOR
      if (isEditor) {
        UnityEditor.EditorGUI.indentLevel = level;
        return;
      }
#endif
    }

    void BoltInternal.IDebugDrawer.Label(string text) {
#if UNITY_EDITOR
      if (isEditor) {
        GUILayout.Label(text);
        return;
      }
#endif

      Bolt.DebugInfo.Label(text);
    }

    void BoltInternal.IDebugDrawer.LabelBold(string text) {
#if UNITY_EDITOR
      if (isEditor) {
        GUILayout.Label(text, EditorStyles.boldLabel);
        return;
      }
#endif

      Bolt.DebugInfo.LabelBold(text);
    }

    void BoltInternal.IDebugDrawer.LabelField(string text, object value) {
#if UNITY_EDITOR
      if (isEditor) {
        UnityEditor.EditorGUILayout.LabelField(text, value.ToString());
        return;
      }
#endif

      Bolt.DebugInfo.LabelField(text, value);
    }

    void BoltInternal.IDebugDrawer.Separator() {
#if UNITY_EDITOR
      if (isEditor) {
        UnityEditor.EditorGUILayout.Separator();
        return;
      }
#endif

      GUILayout.Space(2);
    }

  }
}