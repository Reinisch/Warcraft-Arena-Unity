using System.Diagnostics;

using Debug = UnityEngine.Debug;

namespace Common
{
    [DebuggerStepThrough]
    public static class Logging
    {
        private const string SpellLoggingDefine = "LOG_SPELLS";
        private const string AuraLoggingDefine = "LOG_AURAS";

        [Conditional(SpellLoggingDefine)]
        public static void LogSpell(string message) => Debug.Log(message);

        [Conditional(AuraLoggingDefine)]
        public static void LogAura(string message) => Debug.Log(message);

        [Conditional(SpellLoggingDefine)]
        public static void LogSpellError(string message) => Debug.LogError(message);
    }
}
