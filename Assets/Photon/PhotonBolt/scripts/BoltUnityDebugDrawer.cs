using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BoltInternal
{
	public class UnityDebugDrawer : IDebugDrawer
	{
		bool isEditor;

		void IDebugDrawer.IsEditor(bool isEditor)
		{
			this.isEditor = isEditor;
		}

		void IDebugDrawer.SelectGameObject(GameObject gameObject)
		{
#if UNITY_EDITOR
			if (!isEditor)
			{
				Selection.activeGameObject = gameObject;
			}
#endif
		}

		void IDebugDrawer.Indent(int level)
		{
#if UNITY_EDITOR
			if (isEditor)
			{
				EditorGUI.indentLevel = level;
				return;
			}
#endif
		}

		void IDebugDrawer.Label(string text)
		{
#if UNITY_EDITOR
			if (isEditor)
			{
				GUILayout.Label(text);
				return;
			}
#endif

			Bolt.DebugInfo.Label(text);
		}

		void IDebugDrawer.LabelBold(string text)
		{
#if UNITY_EDITOR
			if (isEditor)
			{
				GUILayout.Label(text, EditorStyles.boldLabel);
				return;
			}
#endif

			Bolt.DebugInfo.LabelBold(text);
		}

		void IDebugDrawer.LabelField(string text, object value)
		{
#if UNITY_EDITOR
			if (isEditor)
			{
				EditorGUILayout.LabelField(text, value.ToString());
				return;
			}
#endif

			Bolt.DebugInfo.LabelField(text, value);
		}

		void IDebugDrawer.Separator()
		{
#if UNITY_EDITOR
			if (isEditor)
			{
				EditorGUILayout.Separator();
				return;
			}
#endif

			GUILayout.Space(2);
		}

		void IDebugDrawer.DrawObjectArray(IDebugDrawerObjectArray root)
		{
#if UNITY_EDITOR
			if (isEditor)
			{
				var fields = root.GetChildren();

				for (int i = 0; i < fields.Length; i++)
				{
					DrawObjectArrayItem(fields[i]);
				}

				return;
			}
#endif
		}

		private void DrawObjectArrayItem(IDebugDrawerObjectArray item)
		{
#if UNITY_EDITOR
			var fields = item.GetChildren();

			if (fields.Length > 0)
			{
				item.IsVisible = EditorGUILayout.Foldout(item.IsVisible, item.GetName(), true);

				if (item.IsVisible)
				{
					EditorGUI.indentLevel++;

					for (int i = 0; i < fields.Length; i++)
					{
						DrawObjectArrayItem(fields[i]);
					}

					EditorGUI.indentLevel--;
				}
			}
			else
			{
				(this as IDebugDrawer).LabelField(item.GetName(), item.GetValue());
			}
#endif
		}
	}
}