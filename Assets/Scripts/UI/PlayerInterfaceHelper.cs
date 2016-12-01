using UnityEngine;
using System.Collections;

public static class PlayerInterfaceHelper
{
    public static void DrawShadow(Rect rect, string text, GUIStyle style, Color txtColor, Color shadowColor, Vector2 direction)
    {
        GUIStyle backupStyle = style;
        GUI.depth = -20;
        style.normal.textColor = shadowColor;
        rect.x += direction.x;
        rect.y += direction.y;
        GUI.Label(rect, text, style);

        style.normal.textColor = txtColor;
        rect.x -= direction.x;
        rect.y -= direction.y;
        GUI.Label(rect, text, style);

        style = backupStyle;
    }
}
