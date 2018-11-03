using System;

namespace Core
{
    public struct ScriptAction
    {
        public Guid SourceGUID;
        public Guid TargetGUID;
        public Guid OwnerGUID;                      // owner of source if source is item
        public ScriptInfo Script;                   // pointer to static script data
    }
}