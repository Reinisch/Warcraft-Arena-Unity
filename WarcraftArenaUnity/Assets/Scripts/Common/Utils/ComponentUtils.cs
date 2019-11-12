using UnityEngine;

namespace Common
{
    public static class ComponentUtils
    {
        private static string GetPath(this Transform current)
        {
            return current.parent == null ? $"/{current.name}" : $"{current.parent.GetPath()}/{current.name}";
        }

        public static string GetPath(this Component component)
        {
            return $"{component.transform.GetPath()}/{component.GetType()}";
        }
    }
}
