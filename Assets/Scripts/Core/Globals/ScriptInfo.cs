using System;

namespace Core
{
    public abstract class ScriptInfo
    {
        public uint ID { get; set; }
        public int Delay { get; set; }
        public ScriptsType Type { get; set; }
        public abstract ScriptCommands Command { get; }

        public string DebugInfo { get { throw new NotImplementedException(); } }
    }
}