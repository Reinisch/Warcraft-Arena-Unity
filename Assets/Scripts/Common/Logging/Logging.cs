using System.Diagnostics;

using Debug = UnityEngine.Debug;

namespace Common
{
    [DebuggerStepThrough]
    public static class Logging
    {
        private const string SpellLoggingDefine = "LOG_SPELLS";

        [Conditional(SpellLoggingDefine)]
        public static void LogSpell(string message) => Debug.Log(message);

        [Conditional(SpellLoggingDefine)]
        public static void LogSpellError(string message) => Debug.LogError(message);
    }
}
