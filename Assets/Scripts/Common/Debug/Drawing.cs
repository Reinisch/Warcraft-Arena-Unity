using System.Diagnostics;
using UnityEngine;

namespace Common
{
    public static class Drawing
    {
        private const string DrawingDefine = "ENABLE_DRAWING";

        [Conditional(DrawingDefine)]
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration) => UnityEngine.Debug.DrawLine(start, end, color, duration);
    }
}
